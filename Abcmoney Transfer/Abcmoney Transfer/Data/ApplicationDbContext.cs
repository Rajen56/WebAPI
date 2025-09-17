using Abcmoney_Transfer.Models;
using static Abcmoney_Transfer.Models.IIdentity;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Transactions;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using static Abcmoney_Transfer.Models.IdentityModel;

namespace Abcmoney_Transfer.Data
{
    public class ApplicationDbContex:

         IdentityDbContext<AppUser, AppRole, int, AppUserClaim, AppUserRole, AppUserLogin, AppRoleClaim, AppUserToken>
    {
        public ApplicationDbContex(DbContextOptions<AbcmoneyTransfer.Models.ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Seedstatus> SeedStatuses { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
    }
}
