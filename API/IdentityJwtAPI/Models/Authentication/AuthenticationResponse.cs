using System.Collections.Generic;

namespace IdentityJwtAPI.Models.Authentication
{
    public class AuthenticationResponse
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string JwtToken { get; set; }
        public string JwtExpiration { get; set; }
        public string RefreshToken { get; set; }

        public IList<Role> Roles { get; set; }
    }
}
