using Library.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace OrderAPI.Models
{
    public class OrderDbContext : DbContext
    {
        public DbSet<Order> Orders { get; set; }

        public OrderDbContext(DbContextOptions<OrderDbContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Order>()
                .Property(e => e.DeliveryDate)
                .HasConversion(
                    d => d.ToDateTime(TimeOnly.MinValue),
                    d => DateOnly.FromDateTime(d)
                );
        }

    }
}
