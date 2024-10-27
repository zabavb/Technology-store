using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Models
{
    public class User
    {
        private long Id { get; set; }
        private string Username { get; set; } = string.Empty;
        private string? FirstName { get; set; } = string.Empty;
        private string? LastName { get; set; } = string.Empty;
        private int? Age { get; set; }
        private string Email { get; set; } = string.Empty;
        private string Phone { get; set; } = string.Empty;
        private string Password { get; set; } = string.Empty;
        private RoleType Role { get; set; } = RoleType.Guest;

        public User(long id, string username, string? firstName, string? lastName, int? age, string email, string phone, string password, RoleType role)
        {
            Id = id;
            Username = username;
            FirstName = firstName;
            LastName = lastName;
            Age = age;
            Email = email;
            Phone = phone;
            Password = password;
            Role = role;
        }

        public User() { }
    }
}
