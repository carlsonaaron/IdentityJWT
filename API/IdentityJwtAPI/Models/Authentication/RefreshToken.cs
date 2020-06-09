using Microsoft.AspNetCore.Identity;
using System;

namespace IdentityJwtAPI.Models.Authentication
{
    public class RefreshToken
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string Token { get; set; }
        public DateTime ExpirationDateTime { get; set; }
        public DateTime IssuedDateTime { get; set; }        
        
        public virtual IdentityUser User { get; set; } 
    }
}
