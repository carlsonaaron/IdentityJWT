using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityJwtAPI.Models.Authentication;
using IdentityJwtAPI.Services;
using IdentityJwtAPI.Services.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static IdentityJwtAPI.ViewModels.AccountViewModels;

namespace IdentityJwtAPI.Controllers.Account
{
    [Route("[controller]")]
    [Authorize]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private IAuthService authService;
        private readonly IRefreshTokenService refreshTokenService;
        private readonly IRolesService rolesService;

        public AccountController(IAuthService authService, IRefreshTokenService refreshTokenService, IRolesService rolesService)
        {
            this.authService = authService;
            this.refreshTokenService = refreshTokenService;
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

                var user = await authService.GetIdentityUserByEmail(model.Email);
                
                var newRefreshToken = refreshTokenService.GenerateNewRefreshToken(user.Id);
                await refreshTokenService.RemoveExistingUserRefreshTokens(user.Id);
                await refreshTokenService.SaveRefreshToken(newRefreshToken);

                var response = new AuthenticationResponse
                {
                    Id = user.Id,
                    Email = model.Email,
                    Roles = await rolesService.GetRolesByUserId(user.Id),
                    RefreshToken = newRefreshToken.Token
                };

                response.JwtToken = await authService.GenerateJwtToken(response);

                return Ok(response);
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
            var userId = principalFromExpiredAccessToken.FindFirstValue(ClaimTypes.NameIdentifier);

            var tokenIsValid = await refreshTokenService.ValidateRefreshToken(userId, request.RefreshToken);
            
            if (!tokenIsValid)
                return Ok(null);

            var newRefreshToken = refreshTokenService.GenerateNewRefreshToken(userId);
            await refreshTokenService.RemoveExistingUserRefreshTokens(userId);
            await refreshTokenService.SaveRefreshToken(newRefreshToken);

            var response = new AuthenticationResponse
            {
                Id = userId,
                Email = email,
                Roles = await rolesService.GetRolesByUserId(userId),
                RefreshToken = newRefreshToken.Token
            };

            response.JwtToken = await authService.GenerateJwtToken(response);

            return Ok(response);
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
