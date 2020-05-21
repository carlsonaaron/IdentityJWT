using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JwtIdentityAPI.Services.Authentication
{
    public interface IRolesService
    {
        public Task<IEnumerable<string>> GetRolesByUserId(string userId);
    }


    public class RolesService : IRolesService
    {
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly UserManager<IdentityUser> userManager;

        public RolesService(RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
        }

        public async Task<IEnumerable<string>> GetRolesByUserId(string userId)
        {
            var identityUser = await userManager.FindByNameAsync(userId);
            var roles = await userManager.GetRolesAsync(identityUser);
            return roles;
        }
    }
}
