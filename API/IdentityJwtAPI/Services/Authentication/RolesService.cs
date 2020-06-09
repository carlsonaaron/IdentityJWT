using IdentityJwtAPI.Models.Authentication;
using JwtIdentityAPI.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityJwtAPI.Services.Authentication
{
    public interface IRolesService
    {
        Task<IList<Role>> GetAll();
        Task<IList<Role>> GetRolesByUserId(string userId);
    }


    public class RolesService : IRolesService
    {
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly UserManager<IdentityUser> userManager;
        private readonly IdentityContext context;

        public RolesService(RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager, IdentityContext context)
        {

            this.roleManager = roleManager;
            this.userManager = userManager;
            this.context = context;
        }

        public async Task<IList<Role>> GetAll()
        {
            var roles = await context.Roles.Select(r => new Role { Id = r.Id, Name = r.Name }).ToListAsync();
            return roles;
        }

        public async Task<IList<Role>> GetRolesByUserId(string userId)
        {
            if (userId == null)
                return null;

            var identityUser = await userManager.FindByIdAsync(userId);
            var roleNames = await userManager.GetRolesAsync(identityUser);

            var identityRoles = new List<IdentityRole>();
            foreach(var roleName in roleNames)
            {
                var identityRole = await roleManager.FindByNameAsync(roleName);
                
                if (identityRole != null)
                    identityRoles.Add(identityRole);
            }

            var roles = identityRoles.Select(r => new Role { Id = r.Id, Name = r.Name }).ToList();
            return roles;
        }

        public async Task<Role> Create(Role newRole)
        {
            var newIdentityRole = new IdentityRole { Name = newRole.Name };
            var identityResult = await roleManager.CreateAsync(newIdentityRole);

            if (!identityResult.Succeeded)
                return null;
            
            newRole.Id = newIdentityRole.Id;
            return newRole;
        }

        public async Task<bool> Update(Role updatedRole)
        {
            var existingIdentityRole = await roleManager.FindByIdAsync(updatedRole.Id);

            if (existingIdentityRole == null)
                return false;

            existingIdentityRole.Name = updatedRole.Name;
            var identityResult = await roleManager.UpdateAsync(existingIdentityRole);

            return identityResult.Succeeded;
        }

        public async Task<bool> Delete(string roleId)
        {
            var existingIdentityRole = await roleManager.FindByIdAsync(roleId);
            
            if (existingIdentityRole == null)
                return false;

            // TODO: Delete Associated UserRoles?  (not sure if this is handled automatically through the role manager.... I doubt it though)
            var identityResult = await roleManager.DeleteAsync(existingIdentityRole);

            return identityResult.Succeeded;
        }
    }
}
