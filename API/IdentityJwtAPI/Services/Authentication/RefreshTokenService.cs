using IdentityJwtAPI.Models.Authentication;
using JwtIdentityAPI.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace IdentityJwtAPI.Services.Authentication
{
    public interface IRefreshTokenService
    {
        Task<IList<RefreshToken>> GetAll();
        Task<IList<RefreshToken>> GetRefreshTokensByUserId(string userId);
        Task<RefreshToken> SaveRefreshToken(RefreshToken token);
        Task<bool> RemoveExistingUserRefreshTokens(string userId);
        Task<bool> DeleteRefreshToken(string tokenId);
        Task<bool> ValidateRefreshToken(string userId, string refreshToken);
        RefreshToken GenerateNewRefreshToken(string userId);
    }


    public class RefreshTokenService: IRefreshTokenService
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly IConfiguration config;
        private readonly JwtSettings jwtSettings;
        private readonly IdentityContext context;

        
        public RefreshTokenService (IdentityContext context, IOptions<JwtSettings> jwtSettings, UserManager<IdentityUser> userManager, IConfiguration config)
        {
            this.context = context;
            this.jwtSettings = jwtSettings.Value;
            this.userManager = userManager;
            this.config = config;
        }

        public async Task<IList<RefreshToken>> GetAll()
        {
            var tokens = await context.RefreshTokens.Include(rt => rt.User).ToListAsync();
            return tokens;
        }

        public async Task<IList<RefreshToken>> GetRefreshTokensByUserId(string userId)
        {
            var tokens = await context.RefreshTokens.Where(rt => rt.UserId == userId).ToListAsync();
            return tokens;
        }

        public async Task<RefreshToken> SaveRefreshToken(RefreshToken token)
        {
            context.RefreshTokens.Add(token);
            await context.SaveChangesAsync();
            return token;
        }

        public async Task<bool> RemoveExistingUserRefreshTokens(string userId)
        {
            var tokens = await GetRefreshTokensByUserId(userId);
            if (tokens.Count > 0)
            {
                context.RefreshTokens.RemoveRange(tokens);
                var tokensDeleted = await context.SaveChangesAsync();
            }
            return true;
        }

        public async Task<bool> DeleteRefreshToken(string tokenId)
        {
            var token = await context.RefreshTokens.SingleOrDefaultAsync(rt => rt.Id == new Guid(tokenId));
            if (token == null)
                return false;

            context.RefreshTokens.Remove(token);
            var deletedItems = await context.SaveChangesAsync();
            return deletedItems == 1;
        }

        public async Task<bool> ValidateRefreshToken(string userId, string refreshToken)
        {
            var existingRefreshTokens = await GetRefreshTokensByUserId(userId);
            var now = DateTime.UtcNow;
            
            var matchingToken = existingRefreshTokens?.FirstOrDefault(rt => rt.Token == refreshToken && now < rt.ExpirationDateTime);
            return matchingToken != null;
        }

        public RefreshToken GenerateNewRefreshToken(string userId)
        {
            using (var randomNumberGenerator = RandomNumberGenerator.Create())
            {
                var randomNumber = new byte[32];

                randomNumberGenerator.GetBytes(randomNumber);
                var tokenString = Convert.ToBase64String(randomNumber);

                var refreshToken = new RefreshToken
                {
                    UserId = userId,
                    ExpirationDateTime = DateTime.UtcNow.AddMinutes(config.GetValue<double>("JwtSettings:RefreshTokenExpireMinutes")),
                    IssuedDateTime = DateTime.UtcNow,
                    Token = tokenString
                };

                return refreshToken;
            }
        }
       
    }
}
