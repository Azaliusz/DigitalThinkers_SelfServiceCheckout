using Microsoft.EntityFrameworkCore;
using SelfServiceCheckout.Models;

namespace SelfServiceCheckout.Data
{
    public class SelfServiceCheckoutDbContext : DbContext
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MoneyDenomination>().HasKey(model => new
            {
                model.Currency,
                model.Denomination
            });
        }

        public DbSet<MoneyDenomination> MoneyDenominations { get; set; }
    }
}