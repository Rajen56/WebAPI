using Abcmoney_Transfer.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Transactions;
using static Abcmoney_Transfer.Models.IIdentity;

namespace ABCExchange.Models
{
    public class ApplicationDbContext : IdentityDbContext<Userlogin, AppRole, int, AppUserClaim, AppUserRole, AppUserLogin, AppRoleClaim, AppUserToken>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        // Add DbSet for your custom entities if needed
        public DbSet<SeedStatus> SeedStatuses { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Change the table names for Identity tables
            builder.Entity<Userlogin>().ToTable("IdentityUser");
            builder.Entity<AppRole>().ToTable("IdentityRoles"); // Changed from Identity to AppRole
            builder.Entity<AppUserRole>().ToTable("UserRoles");
            builder.Entity<AppUserClaim>().ToTable("UserClaims");
            builder.Entity<AppUserLogin>().ToTable("UserLogins");
            builder.Entity<AppRoleClaim>().ToTable("RoleClaims");
            builder.Entity<AppUserToken>().ToTable("UserTokens");
            // Configure your custom entities
            builder.Entity<SeedStatus>().ToTable("SeedStatus");
            builder.Entity<Transaction>().ToTable("Transaction")
                .Property(t => t.TransactionId)
                .ValueGeneratedOnAdd();
            // If you have relationships, configure them here
            // Example: builder.Entity<Userlogin>().HasMany(u => u.Transactions).WithOne(t => t.User);
        }
    }
}