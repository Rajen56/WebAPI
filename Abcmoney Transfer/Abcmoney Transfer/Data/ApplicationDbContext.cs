using Abcmoney_Transfer.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.Transactions;
using static Abcmoney_Transfer.Models.IIdentity;

namespace AbcmoneyTransfer.Models
{
    public class ApplicationDbContext
        : IdentityDbContext<AppUser,AppRole,int,AppUserClaim,AppUserRole,AppUserLogin, AppRoleClaim, AppUserToken>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Rename Identity tables
            builder.Entity<Appuser>().ToTable("IdentityUser");
            builder.Entity<AppRole>().ToTable("IdentityRoles");
            builder.Entity<AppUserRole>().ToTable("UserRoles");
            builder.Entity<AppUserClaim>().ToTable("UserClaims");
            builder.Entity<AppUserLogin>().ToTable("UserLogins");
            builder.Entity<AppRoleClaim>().ToTable("RoleClaims");
            builder.Entity<AppUserToken>().ToTable("UserTokens");

            // Your custom tables
            builder.Entity<Seedstatus>().ToTable("SeedStatus");
            builder.Entity<Transaction>()
                   .ToTable("Transaction")
                   .Property(t => t.TransactionId)
                   .ValueGeneratedOnAdd();
        }
        // Add DbSets for other entities
        public DbSet<Seedstatus>SeedStatuses { get; set; }
        public Dbset<Transaction> Transactions { get; set; }
    }
}
