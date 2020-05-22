using JwtIdentityAPI.Models.Account;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static JwtIdentityAPI.ViewModels.AccountViewModels;

namespace JwtIdentityAPI.Services
{
    public interface IAuthService
    {
        Task<IdentityResult> RegisterUser(RegisterViewModel model);
        Task<SignInResult> Login(LoginViewModel model);
        Task<string> GenerateJwtToken(IAccountViewModel model);
        Task<string> GenerateRefreshToken(string userEmail);
        Task<string> GenerateRefreshToken(IdentityUser user);
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
        Task<string> ValidateAndRenewRefreshToken(string userEmail, string refreshToken);
        Task<bool> Logout();
    }

    public class AuthService : IAuthService
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly IConfiguration config;


        // users hardcoded for simplicity, store in a db with hashed passwords in production applications

        private readonly JwtSettings jwtSettings;

        public AuthService(
            IOptions<JwtSettings> jwtSettings,
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            IConfiguration config)
        {
            this.jwtSettings = jwtSettings.Value;
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.config = config;
        }

        public async Task<IdentityUser> GetIdentityUserByEmail(string userEmail)
        {
            var identityUser = await userManager.FindByEmailAsync(userEmail);
            return identityUser;
        }

        public async Task<IdentityResult> RegisterUser(RegisterViewModel model)
        {
            var user = new IdentityUser { UserName = model.Email, Email = model.Email };
            var result = await userManager.CreateAsync(user, model.Password);

            return result;
        }

        public async Task<SignInResult> Login(LoginViewModel model)
        {
            return await signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);
        }

        public async Task<bool> Logout()
        {
            await signInManager.SignOutAsync();
            return true;
        }




        public async Task<string> GenerateJwtToken(IAccountViewModel model)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(config.GetValue<string>("JwtSettings:SecretKey"));

            var claims = await CreateClaims(model);

            var claimsDictionary = new Dictionary<string, object>();
            foreach (var claim in claims)
            {
                claimsDictionary.Add(claim.Type, claim.Value);
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Claims = claimsDictionary,
                Expires = DateTime.UtcNow.AddSeconds(config.GetValue<double>("JwtSettings:TokenExpireSeconds")),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var jwtToken = jwtTokenHandler.CreateToken(tokenDescriptor);

            return jwtTokenHandler.WriteToken(jwtToken);
        }



        public async Task<string> GenerateRefreshToken(string userEmail)
        {
            var identityUser = await userManager.FindByEmailAsync(userEmail);
            return await GenerateRefreshToken(identityUser);
        }

        public async Task<string> GenerateRefreshToken(IdentityUser identityUser)
        {
            await userManager.RemoveAuthenticationTokenAsync(identityUser, "Default", "RefreshToken");
            var newRefreshToken = await userManager.GenerateUserTokenAsync(identityUser, "Default", "RefreshToken");
            await userManager.SetAuthenticationTokenAsync(identityUser, "Default", "RefreshToken", newRefreshToken);

            return newRefreshToken;
        }

        //private string GenerateRefreshToken()
        //{
        //    var randomNumber = new byte[32];

        //    using (var randomNumberGenerator = RandomNumberGenerator.Create())
        //    {
        //        randomNumberGenerator.GetBytes(randomNumber);
        //        return Convert.ToBase64String(randomNumber);
        //    }
        //}

        public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var key = Encoding.ASCII.GetBytes(config.GetValue<string>("JwtSettings:SecretKey"));

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = false,
                ClockSkew = TimeSpan.Zero

            };

            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
            var jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");

            return principal;
        }

        public async Task<string> ValidateAndRenewRefreshToken(string userEmail, string refreshToken)
        {
            var identityUser = await userManager.FindByEmailAsync(userEmail);

            var existingRefreshToken = await userManager.GetAuthenticationTokenAsync(identityUser, "Default", "RefreshToken");

            if (existingRefreshToken == null)
                return null;

            var updatedRefreshToken = await GenerateRefreshToken(identityUser);

            return updatedRefreshToken;

        }

        // ================= 

        private async Task<IList<Claim>> CreateClaims(IAccountViewModel model)
        {
            var claims = new List<Claim>();

            claims.Add(new Claim(ClaimTypes.Name, model.Email));

            var identityUser = await userManager.FindByEmailAsync(model.Email);
            var roles = await userManager.GetRolesAsync(identityUser);

            claims.AddRange(roles.Select(r => new Claim(ClaimsIdentity.DefaultRoleClaimType, r)));


            return claims;
        }
    }

}
