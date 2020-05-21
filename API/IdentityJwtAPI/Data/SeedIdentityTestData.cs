using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JwtIdentityAPI.Statics;
using Microsoft.AspNetCore.Identity;


namespace IdentityJwtAPI.Data
{
    public static class UserAndRoleDataInitializer
    {
        public static void SeedData(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            SeedRoles(roleManager);
            SeedUsers(userManager);
        }

        private static void SeedUsers(UserManager<IdentityUser> userManager)
        {
            var email = "appuser@test.com";
            if (userManager.FindByEmailAsync(email).Result == null)
            {
                IdentityUser user = new IdentityUser
                {
                    UserName = email,
                    Email = email
                };
                
                IdentityResult result = userManager.CreateAsync(user, "P@ssw0rd1!").Result;

                if (result.Succeeded)
                {
                    userManager.AddToRoleAsync(user, Roles.APP_USER).Wait();
                }
            }

            email = "appadmin@test.com";
            if (userManager.FindByEmailAsync(email).Result == null)
            {
                var user = new IdentityUser
                {
                    UserName = email,
                    Email = email
                };

                IdentityResult result = userManager.CreateAsync(user, "P@ssw0rd1!").Result;

                if (result.Succeeded)
                {
                    userManager.AddToRoleAsync(user, Roles.APP_ADMIN).Wait();
                }
            }           
        }

        private static void SeedRoles(RoleManager<IdentityRole> roleManager)
        {
            if (!roleManager.RoleExistsAsync(Roles.APP_USER).Result)
            {
                IdentityRole role = new IdentityRole();
                role.Name = Roles.APP_USER;
                IdentityResult roleResult = roleManager.CreateAsync(role).Result;
            }


            if (!roleManager.RoleExistsAsync(Roles.APP_ADMIN).Result)
            {
                IdentityRole role = new IdentityRole();
                role.Name = Roles.APP_ADMIN;
                IdentityResult roleResult = roleManager.CreateAsync(role).Result;
            }           
        }
    }
}
