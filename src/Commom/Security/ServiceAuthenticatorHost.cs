using System;

namespace Common.Security
{
    public class ServiceAuthenticatorHost : IServiceAuthenticatorHost
    {
        private readonly IJwtTokenHandler _jwtTokenHandler;
        private readonly ISecurityConfiguration _securityConfiguration;

        public ServiceAuthenticatorHost(IJwtTokenHandler jwtTokenHandler,
            ISecurityConfiguration securityConfiguration)
        {
            _jwtTokenHandler = jwtTokenHandler;
            _securityConfiguration = securityConfiguration;
        }

        public JwtBasic CreateToken(Credentials credentials)
        {
            if (string.IsNullOrEmpty(credentials?.Username)
                || string.IsNullOrEmpty(credentials?.Password))
            {
                return null;
            }
            if (credentials.Username.Equals(_securityConfiguration.ServiceSettings.Username)
                && credentials.Password.Equals(_securityConfiguration.ServiceSettings.Password))
            {
                var token = _jwtTokenHandler.Create(credentials.Username, string.Empty,
                    TimeSpan.FromDays(1000));

                return token;
            }

            return null;
        }
    }
}