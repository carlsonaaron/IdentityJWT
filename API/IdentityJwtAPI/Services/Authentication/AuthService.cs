using IdentityJwtAPI.Models.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using static HomePortalAPI.ViewModels.AccountViewModels;

namespace IdentityJwtAPI.Services
{
    public interface IAuthService
    {
        Task<IdentityUser> GetIdentityUserByEmail(string userEmail);
        Task<IdentityResult> RegisterUser(RegisterViewModel model);
        Task<SignInResult> Login(LoginViewModel model);
        Task<string> GenerateJwtToken(AuthenticationResponse model);
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
        Task<bool> Logout();
    }

    public class AuthService : IAuthService
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly IConfiguration config;


        public AuthService(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, IConfiguration config)
        {
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

        public async Task<string> GenerateJwtToken(AuthenticationResponse model)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(config.GetValue<string>("JwtSettings:SecretKey"));

            var claims = await CreateClaims(model);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddSeconds(config.GetValue<double>("JwtSettings:TokenExpireSeconds")),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var jwtToken = jwtTokenHandler.CreateToken(tokenDescriptor);

            return jwtTokenHandler.WriteToken(jwtToken);
        }


        private async Task<IList<Claim>> CreateClaims(AuthenticationResponse model)
        {
            var claims = new List<Claim> {
                new Claim(ClaimTypes.Name, model.Email),
                new Claim(ClaimTypes.NameIdentifier, model.Id.ToString()),
                new Claim(ClaimTypes.Email, model.Email)
            };

            // Add UserRole Claims
            var identityUser = await userManager.FindByEmailAsync(model.Email);
            var roles = await userManager.GetRolesAsync(identityUser);

            claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));
            return claims;
        }
    }

}
