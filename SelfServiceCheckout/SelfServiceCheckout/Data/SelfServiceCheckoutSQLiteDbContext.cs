using Microsoft.EntityFrameworkCore;

namespace SelfServiceCheckout.Data
{
    public class SelfServiceCheckoutSQLiteDbContext : SelfServiceCheckoutDbContext
    {
        public string DbPath { get; }

        public SelfServiceCheckoutSQLiteDbContext()
        {
            DbPath = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "blogging.db");
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite($"Data Source={DbPath}");
    }
}