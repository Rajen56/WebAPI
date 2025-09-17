using Abcmoney_Transfer.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Transactions;
using static Abcmoney_Transfer.Models.IdentityModel;
namespace Abcmoney_Transfer.Data
{
    public class ApplicationDbContext : IdentityDbContext<AppUser, IdentityRole, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        // Add your custom DbSet properties for application-specific entities here
        public DbSet<Seedstatus> SeedStatuses { get; set; }
        public DbSet<Transactioninformation> Transactions { get; set; }
        public DbSet<AppUser> appUsers { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Core Identity model and override any defaults if needed.
            // For example, you can change the table names or add custom properties to Identity entities.
        }
    }

}
