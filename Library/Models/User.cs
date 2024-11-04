﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Models
{
    public class User
    {
        public long Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string? FirstName { get; set; } = string.Empty;
        public string? LastName { get; set; } = string.Empty;
        public int? Age { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string? Role { get; set; } = RoleType.Guest.ToString();
        public List<Product> Basket { get; set; } = new();
        public Order Orders { get; set; } = new();

        public User(long id, string username, string? firstName, string? lastName, int? age, string email, string phone, string password, string? role, List<Product> basket, Order orders)
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
            Basket = basket;
            Orders = orders;
        }

        public User(long id, string username, string? firstName, string? lastName, int? age, string email, string phone, string password, string? role)
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
