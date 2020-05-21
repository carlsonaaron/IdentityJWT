using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JwtIdentityAPI.Statics;
using JwtIdentityAPI.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace JwtIdentityAPI.Controllers.Account
{
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize(Roles = Roles.ADMIN)]
    public class UserRoleController : ControllerBase
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<IdentityUser> _userManager;

        public UserRoleController(RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        [HttpGet("{roleId}")]
        
        public async Task<IActionResult> Get(string roleId)
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


        [HttpPost("{roleId}")]
        public async Task<IActionResult> Post(string roleId, IEnumerable<UserRoleViewModel> model)
        {
            var role = await _roleManager.FindByIdAsync(roleId);

            if (role == null)
            {
                return NotFound("Role was not found");
            }

            foreach (var item in model)
            {
                var user = await _userManager.FindByIdAsync(item.UserId);

                IdentityResult result = null;
                if (item.IsSelected == true && !(await _userManager.IsInRoleAsync(user, role.Name)))
                {
                    result = await _userManager.AddToRoleAsync(user, role.Name);
                }
                else if (!item.IsSelected && !!(await _userManager.IsInRoleAsync(user, role.Name)))
                {
                    result = await _userManager.RemoveFromRoleAsync(user, role.Name);
                }
            }

            return Ok();
        }
    }
}