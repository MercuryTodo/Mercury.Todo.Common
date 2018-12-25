using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Principal;

namespace Common.Security
{
    public class MercuryIdentity : ClaimsPrincipal
    {
        private readonly IEnumerable<Claim> _claims;
        public string Role { get; }
        public string State { get; }
        public override IEnumerable<Claim> Claims => _claims;

        public MercuryIdentity(string name, string role, string state,
            IEnumerable<Claim> claims)
            : base(new GenericIdentity(name))
        {
            Role = role;
            State = state;
            _claims = claims;
        }
    }
}