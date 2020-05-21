using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using JwtIdentityAPI.Models.Account;
using JwtIdentityAPI.Services;
using JwtIdentityAPI.Services.Authentication;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using static JwtIdentityAPI.ViewModels.AccountViewModels;

namespace JwtIdentityAPI.Controllers.Account
{
    [Route("[controller]")]
    [Authorize]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private IAuthService authService;
        private readonly IRolesService rolesService;

        public AccountController(IAuthService authService, IRolesService rolesService)
        {
            this.authService = authService;
            this.rolesService = rolesService;
        }

        [HttpGet("GetCurrentRoles")]
        public async Task<IActionResult> Get()
        {
            var userId = User.FindFirstValue(ClaimTypes.Name);

            var roles = await rolesService.GetRolesByUserId(userId);

            return Ok(roles);
        }

        [HttpPost("Register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody]RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var registerResult = await authService.RegisterUser(model);

          
            if (!registerResult.Succeeded)
            {
                var errors = new List<string>();
                foreach (var error in registerResult.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                return new BadRequestObjectResult(errors);
            }           
            
            return Ok();           
        }

        [HttpPost("Login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody]LoginViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest();
                }

                var signInResult = await authService.Login(model);

                if (!signInResult.Succeeded)
                {
                    return Unauthorized();
                }

                var authenticationResponse = new AuthenticationResponse
                {
                    Email = model.Email,
                    JwtToken = await authService.GenerateJwtToken(model),
                    RefreshToken = await authService.GenerateRefreshToken(model.Email)
                };

                return Ok(authenticationResponse);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
             
        }

        [HttpPost("Refresh")]
        [AllowAnonymous]
        public async Task<IActionResult> Refresh([FromBody] RefreshRequest request)
        {
            var principalFromExpiredAccessToken = authService.GetPrincipalFromExpiredToken(request.AccessToken);
            var email = principalFromExpiredAccessToken.FindFirstValue(ClaimTypes.Name);
            

            var updatedRefreshToken = await authService.ValidateAndRenewRefreshToken(email, request.RefreshToken);

            var model = new AccountViewModel { Email = email };

            var authenticationResponse = new AuthenticationResponse
            {
                Email = email,
                JwtToken = await authService.GenerateJwtToken(model),
                RefreshToken = updatedRefreshToken
            };


            return Ok(authenticationResponse);
        }


        [HttpPost("Logout")]
        [AllowAnonymous]
        public async Task<IActionResult> Logout()
        {
            await authService.Logout();
            return Ok();
        }


        
    }



}