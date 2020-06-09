using System.Collections.Generic;
using System.Threading.Tasks;
using JwtIdentityAPI.Statics;
using JwtIdentityAPI.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HomePortalAPI.Controllers.Account
{
    [ApiController]
    [Route("[controller]")]
    [Authorize(Roles = Roles.APP_ADMIN)]
    public class UserRoleController : ControllerBase
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<IdentityUser> _userManager;

        public UserRoleController(RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }


        [HttpGet("Role/{roleId}")]
        public async Task<IActionResult> GetRoleUsers(string roleId)
        {
            IdentityRole role = await _roleManager.FindByIdAsync(roleId);

            if (role == null)
            {
                return NotFound("Role was not found");
            }

            var roleUsers = new List<UserRoleViewModel>();

            foreach (var user in _userManager.Users)
            {
                var roleUser = new UserRoleViewModel
                {
                    UserId = user.Id,
                    UserName = user.UserName,
                    IsSelected = await _userManager.IsInRoleAsync(user, role.Name)
                };
                roleUsers.Add(roleUser);
            }

            return Ok(roleUsers);
        }

        [HttpGet("User/{userId}")]        
        public async Task<IActionResult> GetUserRoles(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound("Invalid user");

            var allRoles = await _roleManager.Roles.ToListAsync();

            var result = new List<UserRoleViewModel>();            

            foreach (var role in allRoles)
            {
                var userRole = new UserRoleViewModel
                {
                    UserId = user.Id,
                    UserName = user.UserName,
                    RoleId = role.Id,
                    RoleName = role.Name,
                    IsSelected = await _userManager.IsInRoleAsync(user, role.Name)
                };
                result.Add(userRole);
            }

            return Ok(result);
        }


        [HttpPost("User/{userId}")]
        public async Task<IActionResult> Post(string userId, IEnumerable<UserRoleViewModel> model)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound("Invalid user");

            foreach (var item in model)
            {
                IdentityResult result = null;
                if (item.IsSelected == true && !(await _userManager.IsInRoleAsync(user, item.RoleName)))
                {
                    result = await _userManager.AddToRoleAsync(user, item.RoleName);
                }
                else if (!item.IsSelected && !!(await _userManager.IsInRoleAsync(user, item.RoleName)))
                {
                    result = await _userManager.RemoveFromRoleAsync(user, item.RoleName);
                }
            }

            return Ok();
        }
    }
}