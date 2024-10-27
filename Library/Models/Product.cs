using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Models
{
    public class Product
    {
        private int Id { get; }
        private string Brand { get; set; } = string.Empty;
        private string Model { get; set; } = string.Empty;
        private string Name { get; set; } = string.Empty;
        private string Producer { get; set; } = string.Empty;
        private double Price { get; set; }
        private string? Details { get; set; } = string.Empty;

        public Product(int id, string brand, string model, string producer, double price, string? details)
        {
            Id = id;
            Brand = brand;
            Model = model;
            Name = $"{brand} {model}";
            Producer = producer;
            Price = price;
            Details = details;
        }

        public Product() { }
    }
}
