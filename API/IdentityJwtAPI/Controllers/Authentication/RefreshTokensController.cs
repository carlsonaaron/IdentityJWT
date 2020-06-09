using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityJwtAPI.ViewModels;
using IdentityJwtAPI.Models.Authentication;
using IdentityJwtAPI.Services.Authentication;
using JwtIdentityAPI.Statics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdentityJwtAPI.Controllers.Account
{
    [ApiController]
    [Route("[controller]")]
    [Authorize(Roles = Roles.APP_ADMIN)]
    public class RefreshTokensController : ControllerBase
    {
        private readonly IRefreshTokenService refreshTokenService;

        public RefreshTokensController(IRefreshTokenService refreshTokenService)
        {
            this.refreshTokenService = refreshTokenService;
        }

        [HttpGet]
        public async Task<IEnumerable<RefreshTokenViewModel>> Get()
        {
            var refreshTokens = await refreshTokenService.GetAll();
            var response = new List<RefreshTokenViewModel>();
            foreach(var token in refreshTokens)
            {
                response.Add(new RefreshTokenViewModel
                {
                    Id = token.Id,
                    UserId = token.UserId,
                    UserName = token.User.UserName,
                    ExpirationDateTime = token.ExpirationDateTime,
                    IssuedDateTime = token.IssuedDateTime,
                    Token = token.Token
                });
            }
            return response; 
        }

        [HttpGet("{userId}")]
        public async Task<IEnumerable<RefreshToken>> GetByUserId(string userId)
        {
            return await refreshTokenService.GetRefreshTokensByUserId(userId);
        }
        
        [HttpDelete("{id}")]
        public async Task<bool> Delete(string id) 
        {
            return await refreshTokenService.DeleteRefreshToken(id);
        }
    }
}