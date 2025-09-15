using Abcmoney_Transfer.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;
using System.Transactions;

namespace ABCExchange.Models
{
    public class ApplicationDbContext : IdentityDbContext<Userlogin, Identity, int, AppUserClaim, AppUserRole, AppUserLogin, AppRoleClaim, AppUserToken>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Change the table names for Identity tables
            builder.Entity<Userlogin>().ToTable("IdentityUser");
            builder.Entity<Identity>().ToTable("IdentityRoles");
            builder.Entity<AppUserRole>().ToTable("UserRoles");
            builder.Entity<AppUserClaim>().ToTable("UserClaims");
            builder.Entity<AppUserLogin>().ToTable("UserLogins");
            builder.Entity<AppRoleClaim>().ToTable("RoleClaims");
            builder.Entity<AppUserToken>().ToTable("UserTokens");
            builder.Entity<SeedStatus>().ToTable("SeedStatus");
            builder.Entity<Transaction>().ToTable("Transaction").Property(t => t.TransactionId).ValueGeneratedOnAdd();
   
        }
    }
}
