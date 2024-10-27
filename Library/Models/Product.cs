using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Models
{
    public class Product
    {
        public long Id { get; set; }
        public string Brand { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Producer { get; set; } = string.Empty;
        public double Price { get; set; }
        public string? Details { get; set; } = string.Empty;

        public Product(long id, string brand, string model, string producer, double price, string? details)
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
