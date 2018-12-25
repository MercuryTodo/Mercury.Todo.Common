using System.Threading.Tasks;

namespace Common.Security
{
    public interface IServiceAuthenticatorClient
    {
        Task<JwtBasic> AuthenticateAsync(string serviceUrl, Credentials credentials);
    }
}