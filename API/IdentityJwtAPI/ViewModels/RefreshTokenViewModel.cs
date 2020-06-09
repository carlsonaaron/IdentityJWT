using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HomePortalAPI.ViewModels
{
    public class RefreshTokenViewModel
    {
        public Guid Id { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Token { get; set; }
        public DateTime ExpirationDateTime { get; set; }
        public DateTime IssuedDateTime { get; set; }
    }
}
