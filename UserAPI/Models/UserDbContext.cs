using Library.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Data;
using System.Text;

namespace UserAPI.Models
{
    public class UserDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        
        public UserDbContext(DbContextOptions<UserDbContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            string pass1 = "1234", pass2 = "4321", pass3 = "1234";

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:7074/");
                {
                    var content = new StringContent(JsonConvert.SerializeObject(pass1), Encoding.UTF8, "application/json");
                    HttpResponseMessage response = client.PostAsync("gateway/users/hashpassword", content).Result;
                    if (response.IsSuccessStatusCode)
                        pass1 = response.Content.ReadAsStringAsync().Result;
                }

                {
                    var content = new StringContent(JsonConvert.SerializeObject(pass2), Encoding.UTF8, "application/json");
                    HttpResponseMessage response = client.PostAsync("gateway/users/hashpassword", content).Result;
                    if (response.IsSuccessStatusCode)
                        pass2 = response.Content.ReadAsStringAsync().Result;
                }
                {
                    var content = new StringContent(JsonConvert.SerializeObject(pass3), Encoding.UTF8, "application/json");
                    HttpResponseMessage response = client.PostAsync("gateway/users/hashpassword", content).Result;
                    if (response.IsSuccessStatusCode)
                        pass3 = response.Content.ReadAsStringAsync().Result;
                }
            }
            modelBuilder.Entity<User>().HasData(
                new User(1, "zabavb",  "Viktor", "Bilonizhka", 18, "bilonizkavik@gmail.com", "+380000000000", pass1, RoleType.Admin.ToString()),
                new User(2, "olegoovich", "Oleg", "Olegovich", 23, "olegovichmail@gmail.com", "+440000000000", pass2, RoleType.Moderator.ToString()),
                new User(3, "optimus_prime", "Optimus", "Prime", 45, "optimusprimemail@gmail.com", "+910000000000", pass3, RoleType.User.ToString())
            );
            base.OnModelCreating(modelBuilder);
        }
    }
}
