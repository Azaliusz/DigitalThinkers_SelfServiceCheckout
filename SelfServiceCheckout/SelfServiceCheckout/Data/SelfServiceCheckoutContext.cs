﻿using Microsoft.EntityFrameworkCore;
using SelfServiceCheckout.Models;

namespace SelfServiceCheckout.Data
{
    public class SelfServiceCheckoutContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseInMemoryDatabase("SelfServiceCheckout");
        }

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