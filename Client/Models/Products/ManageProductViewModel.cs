using Library.Models;
using System.ComponentModel.DataAnnotations;

namespace Client.Models.Products
{
    public class ManageProductViewModel
    {
        [Required(ErrorMessage = "Brand is required")]
        [StringLength(16, MinimumLength = 2, ErrorMessage = "Brand must be in range between 2 and 16 characters")]
        public string Brand { get; set; } = string.Empty;

        [Required(ErrorMessage = "Model is required")]
        [StringLength(16, MinimumLength = 2, ErrorMessage = "Model must be in range between 2 and 16 characters")]
        public string Model { get; set; } = string.Empty;

        [Required(ErrorMessage = "Producer is required")]
        [StringLength(16, MinimumLength = 2, ErrorMessage = "Producer must be in range between 2 and 16 characters")]
        public string Producer { get; set; } = string.Empty;

        [Required(ErrorMessage = "Price is required")]
        [Range(0, 2000000, ErrorMessage = "Price must be in range between 0 and 2.000.000")]
        public double Price { get; set; }

        [StringLength(512, MinimumLength = 16, ErrorMessage = "Producer must be in range between 16 and 512 characters")]
        public string? Details { get; set; } = string.Empty;

        public ManageProductViewModel(long id, string brand, string model, string producer, double price, string? details)
        {
            Id = id;
            Brand = brand;
            Model = model;
            Producer = producer;
            Price = price;
            Details = details;
        }

        public ManageProductViewModel(Product product)
        {
            Id = product.Id;
            Brand = product.Brand;
            Model = product.Model;
            Producer = product.Producer;
            Price = product.Price;
            Details = product.Details;
        }

        public ManageProductViewModel() { }
    }
}
