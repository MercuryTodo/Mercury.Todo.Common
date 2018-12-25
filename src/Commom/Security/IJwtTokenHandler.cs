using System;

namespace Common.Security
{
    public interface IJwtTokenHandler
    {
        JwtDetails Parse(string token);

        JwtBasic Create(
            string userId,
            string role,
            TimeSpan? expiry = null,
            string state = "active");

        string GetFromAuthorizationHeader(string authorizationHeader);
    }
}