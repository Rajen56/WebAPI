using Abcmoney_Transfer.Data;

namespace AbcmoneyTransfer.Models
{
    public class IdentityDbContext<T1, T2, T3, T4, T5, T6, T7, T8>
    {
        private DbContextOptions<ApplicationDbContext> options;

        public IdentityDbContext(DbContextOptions<ApplicationDbContext> options)
        {
            this.options = options;
        }
    }
}