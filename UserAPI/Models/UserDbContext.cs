using Library.Models;
using Microsoft.EntityFrameworkCore;

namespace UserAPI.Models
{
    public class UserDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        
        public UserDbContext(DbContextOptions<UserDbContext> options) : base(options)
        {
            Database.EnsureCreated();
        }
    }
}
