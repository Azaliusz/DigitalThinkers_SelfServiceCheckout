using Microsoft.EntityFrameworkCore;

namespace SelfServiceCheckout.Data
{
    public class SelfServiceCheckoutInMemoryDbContext : SelfServiceCheckoutDbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseInMemoryDatabase("SelfServiceCheckout");
        }
    }
}