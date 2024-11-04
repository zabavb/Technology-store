using Library.Models;
using System.ComponentModel.DataAnnotations;

namespace Client.Models.Orders
{
    public class OrderViewModel
    {
        public List<Product> Items { get; set; } = new();

        [Display(Name = "Receiver name")]
        [Required(ErrorMessage = "Name of receiver is required")]
        public string ReceiverName { get; set; } = string.Empty;

        [Display(Name = "Receiver email")]
        [Required(ErrorMessage = "Email of receiver is required")]
        public string ReceiverEmail { get; set; } = string.Empty;

        [Display(Name = "Receiver phone")]
        [Required(ErrorMessage = "Phone of receiver is required")]
        public string ReceiverPhone { get; set; } = string.Empty;

        [Required(ErrorMessage = "Country is required")]
        public string Country { get; set; } = string.Empty;

        [Required(ErrorMessage = "Locality is required")]
        public string Locality { get; set; } = string.Empty;
        
        [Display(Name = "Receiver address")]
        [Required(ErrorMessage = "Address of receiver is required")]
        public string Address { get; set; } = string.Empty;

        [ScaffoldColumn(false)]
        public string DeliveryDate { get; set; } = DateOnly.FromDateTime(DateTime.Now).ToString();
    }
}
