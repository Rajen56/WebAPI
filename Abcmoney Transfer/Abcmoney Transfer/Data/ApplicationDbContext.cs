using Abcmoney_Transfer.Models;
using static Abcmoney_Transfer.Models.IIdentity;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Transactions;

namespace AbcmoneyTransfer.Models
{
    public class ApplicationDbContext
        : IdentityDbContext<AppUser, AppRole, int, AppUserClaim, AppUserRole, AppUserLogin, AppRoleClaim, AppUserToken>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Rename Identity tables
            builder.Entity<AppUser>().ToTable("IdentityUser");
            builder.Entity<AppRole>().ToTable("IdentityRoles");
            builder.Entity<AppUserRole>().ToTable("UserRoles");
            builder.Entity<AppUserClaim>().ToTable("UserClaims");
            builder.Entity<AppUserLogin>().ToTable("UserLogins");
            builder.Entity<AppRoleClaim>().ToTable("RoleClaims");
            builder.Entity<AppUserToken>().ToTable("UserTokens");

            // Custom tables
            builder.Entity<SeedStatus>().ToTable("SeedStatus");
            builder.Entity<Transaction>()
                   .ToTable("Transaction")
            .Property(t => t.TransactionId)
                   .ValueGeneratedOnAdd();
        }

        // DbSets for your custom entities
        public DbSet<SeedStatus> SeedStatuses { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
    }
}
