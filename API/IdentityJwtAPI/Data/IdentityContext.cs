using IdentityJwtAPI.Models.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace JwtIdentityAPI.Data
{
    public class IdentityContext : IdentityDbContext
    {

        public IdentityContext(DbContextOptions<IdentityContext> options) : base(options) { }

        public DbSet<RefreshToken> RefreshTokens { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RefreshToken>().ToTable("RefreshToken");

            base.OnModelCreating(modelBuilder);
        }
    }
}
