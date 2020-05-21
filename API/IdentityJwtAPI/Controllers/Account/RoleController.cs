using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JwtIdentityAPI.Attributes;
using JwtIdentityAPI.Statics;
using JwtIdentityAPI.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JwtIdentityAPI.Controllers.Account
{
    [ApiController]
    [Route("[controller]")]
    public class RoleController : ControllerBase
    {
        private readonly RoleManager<IdentityRole> _roleManager;


        public RoleController(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }

        [HttpGet]
        [Authorize(Roles = Roles.APP_ADMIN)]
        public async Task<IEnumerable<IdentityRole>> Get()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            return roles;
        }

        [HttpGet("AppAdmin")]
        [Authorize(Roles = Roles.APP_ADMIN)]
        public async Task<IEnumerable<IdentityRole>> GetForAppAdmin()
        {
            var roles = await _roleManager.Roles.Where(r => r.Name == Roles.APP_ADMIN).ToListAsync();
            return roles;
        }

        [HttpGet("AppUser")]
        [Authorize(Roles = Roles.APP_USER)]
        public async Task<IEnumerable<IdentityRole>> GetForAppUser()
        {
            var roles = await _roleManager.Roles.Where(r => r.Name == Roles.APP_USER).ToListAsync();
            return roles;
        }

        [HttpGet("{id}")]
        public async Task<IdentityRole> Get(string id)
        {
            var role = await _roleManager.Roles.SingleOrDefaultAsync(r => r.Id == id);
            return role;
        }

        [HttpPost]
        public async Task<bool> Post(CreateRoleViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return false;
            }
            var identityRole = new IdentityRole
            {
                Name = model.RoleName
            };

            var result = await _roleManager.CreateAsync(identityRole);

            return result.Succeeded;
        }




    }
}