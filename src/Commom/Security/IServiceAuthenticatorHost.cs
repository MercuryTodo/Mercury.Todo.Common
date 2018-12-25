namespace Common.Security
{
    public interface IServiceAuthenticatorHost
    {
        JwtBasic CreateToken(Credentials credentials);
    }
}