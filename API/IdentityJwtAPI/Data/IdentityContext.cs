using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace JwtIdentityAPI.Data
{
    public class IdentityContext : IdentityDbContext
    {

        public IdentityContext(DbContextOptions<IdentityContext> options) : base(options) { }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {


            base.OnModelCreating(modelBuilder);
        }
    }
}
