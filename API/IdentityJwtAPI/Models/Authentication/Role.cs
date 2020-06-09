using System;

namespace IdentityJwtAPI.Models.Authentication
{
    public class Role
    {
        public string Id { get; set; }
        public string Name { get; set; }

        internal object ToListAsync()
        {
            throw new NotImplementedException();
        }
    }
}
