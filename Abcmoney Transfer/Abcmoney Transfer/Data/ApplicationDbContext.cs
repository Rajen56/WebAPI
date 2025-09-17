using Abcmoney_Transfer.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Transactions;
using static Abcmoney_Transfer.Models.IdentityModel;
namespace Abcmoney_Transfer.Data
{
    public class ApplicationDbContex: IdentityDbContext<IdentityUser, IdentityRole>
    {
        public ApplicationDbContex(DbContextOptions options)
            : base(options) { }
        public DbSet<Seedstatus> SeedStatuses { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
    }
}
