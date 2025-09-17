using Abcmoney_Transfer.Data;
using Microsoft.EntityFrameworkCore;

namespace AbcmoneyTransfer.Models
{
    public class IdentityDbContext<T1, T2, T3, T4, T5, T6, T7, T8>
    {
        private readonly DbContextOptions options;

        public IdentityDbContext(DbContextOptions options)
        {
            this.options = options;
        }
    }
}