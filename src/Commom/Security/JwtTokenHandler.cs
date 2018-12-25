using Common.Extensions;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using ThirdParty.BouncyCastle.OpenSsl;

namespace Common.Security
{
    public class JwtTokenHandler : IJwtTokenHandler
    {
        private static readonly string RoleClaim = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role";
        private static readonly string StateClaim = "state";
        private readonly ISecurityConfiguration _settings;
        private readonly JwtSecurityTokenHandler _jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
        private TokenValidationParameters _tokenValidationParameters;
        private SecurityKey _issuerSigningKey;
        private SigningCredentials _signingCredentials;
        private JwtHeader _jwtHeader;
        private ILogger _logger { get; }

        public JwtTokenHandler(ISecurityConfiguration settings, ILogger logger)
        {
            _logger = logger;
            _settings = settings;
            if (_settings.JwtTokenSettings.UseRsa)
            {
                InitializeRsa();
            }
            else
            {
                InitializeHmac();
            }
            InitializeJwtParameters();
        }

        private void InitializeRsa()
        {
            using (RSA publicRsa = RSA.Create())
            {
                var publicKeyXml = _settings.JwtTokenSettings.UseRsaFilePath ?
                    System.IO.File.ReadAllText(_settings.JwtTokenSettings.RsaPublicKeyXML) :
                    _settings.JwtTokenSettings.RsaPublicKeyXML;
                RSACryptoServiceProviderExtensions.FromXmlString(publicRsa, publicKeyXml);
                _issuerSigningKey = new RsaSecurityKey(publicRsa);
            }
            if (string.IsNullOrEmpty(_settings.JwtTokenSettings.RsaPrivateKey))
            {
                return;
            }
            using (RSA privateRsa = RSA.Create())
            {
                if (_settings.JwtTokenSettings.UseRsaFilePath)
                {
                    using (var streamReader = File.OpenText(_settings.JwtTokenSettings.RsaPrivateKey))
                    {
                        var pemReader = new PemReader(streamReader);
                        privateRsa.ImportParameters(pemReader.ReadPrivatekey());
                    }
                }
                else
                {
                    using (var stringReader = new StringReader(_settings.JwtTokenSettings.RsaPrivateKey))
                    {
                        var pemReader = new PemReader(stringReader);
                        privateRsa.ImportParameters(pemReader.ReadPrivatekey());
                    }
                }
                var privateKey = new RsaSecurityKey(privateRsa);
                _signingCredentials = new SigningCredentials(privateKey, SecurityAlgorithms.RsaSha256);
            }
        }

        private void InitializeHmac()
        {
            _issuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.JwtTokenSettings.SecretKey));
            _signingCredentials = new SigningCredentials(_issuerSigningKey, SecurityAlgorithms.HmacSha256);
        }

        private void InitializeJwtParameters()
        {
            _jwtHeader = new JwtHeader(_signingCredentials);
            _tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidIssuer = _settings.JwtTokenSettings.Issuer,
                ValidateIssuer = _settings.JwtTokenSettings.ValidateIssuer,
                IssuerSigningKey = _issuerSigningKey
            };
        }

        public JwtBasic Create(
            string userId,
            string role,
            TimeSpan? expiry = null,
            string state = "active")
        {
            var now = DateTime.UtcNow;
            var claims = new Claim[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId),
                new Claim(JwtRegisteredClaimNames.UniqueName, userId),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, now.ToTimestamp().ToString()),
                new Claim(ClaimTypes.Role, role),
                new Claim(StateClaim, state)
            };
            var expires = now.AddDays(_settings.JwtTokenSettings.ExpiryDays);
            var jwt = new JwtSecurityToken(
                issuer: _settings.JwtTokenSettings.Issuer,
                claims: claims,
                notBefore: now,
                expires: expires,
                signingCredentials: _signingCredentials
            );
            var token = new JwtSecurityTokenHandler().WriteToken(jwt);

            return new JwtBasic
            {
                Token = token,
                Expires = expires.ToTimestamp()
            };
        }

        public string GetFromAuthorizationHeader(string authorizationHeader)
        {
            if (string.IsNullOrWhiteSpace(authorizationHeader))
            {
                return null;
            }
            var data = authorizationHeader.Trim().Split(' ');
            if (data.Length != 2 || data.Any(x => string.IsNullOrWhiteSpace(x)))
            {
                return null;
            }
            if (!string.Equals(data[0], "bearer", StringComparison.InvariantCultureIgnoreCase))
            {
                return null;
            }

            return data[1];
        }

        public JwtDetails Parse(string token)
        {
            SecurityToken validatedSecurityToken = null;
            try
            {
                _jwtSecurityTokenHandler.ValidateToken(token, _tokenValidationParameters, out validatedSecurityToken);
                var validatedJwt = validatedSecurityToken as JwtSecurityToken;

                return new JwtDetails
                {
                    Subject = validatedJwt.Subject,
                    Claims = validatedJwt.Claims,
                    Role = validatedJwt.Claims.FirstOrDefault(x => x.Type == RoleClaim)?.Value,
                    State = validatedJwt.Claims.FirstOrDefault(x => x.Type == StateClaim)?.Value,
                    Expires = validatedJwt.ValidTo.ToTimestamp()
                };
            }
            catch (Exception exception)
            {
                _logger.Error(exception, $"JWT Token parser error. {exception.Message}");

                return null;
            }
        }
    }
}