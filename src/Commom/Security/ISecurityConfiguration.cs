namespace Common.Security
{
    public interface ISecurityConfiguration
    {
        JwtTokenSettings JwtTokenSettings { get; }

        ServicesSettings ServicesSettings { get; }

        ServiceSettings ServiceSettings { get; }
    }
}