using Library.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Data;
using System.Text;

namespace ProductAPI.Models
{
    public class ProductDbContext : DbContext
    {
        public DbSet<Product> Employees { get; set; }

        public ProductDbContext(DbContextOptions<ProductDbContext> options) : base(options)
        {
            Database.EnsureCreated();
        }
    }
}
