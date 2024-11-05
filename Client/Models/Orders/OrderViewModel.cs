using Library.Models;
using System.ComponentModel.DataAnnotations;

namespace Client.Models.Orders
{
    public class OrderViewModel
    {
        public long Id { get; set; }
        public List<Product> Items { get; set; } = new();

        [Display(Name = "Receiver username")]
        [Required(ErrorMessage = "Username of receiver is required")]
        public string ReceiverUsername { get; set; } = string.Empty;

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
        public DateOnly DeliveryDate { get; set; } = DateOnly.FromDateTime(DateTime.Now.AddDays(2));

        public OrderViewModel(List<Product> items, User receiver)
        {
            Items = items;
            ReceiverUsername = receiver.Username;
            ReceiverName = receiver.FirstName + receiver.LastName;
            ReceiverEmail = receiver.Email;
            ReceiverPhone = receiver.Phone;
        }

        public OrderViewModel(Order order)
        {
            Items = order.Items;
            ReceiverUsername = order.Receiver.Username;
            ReceiverName = order.Receiver.FirstName + order.Receiver.LastName;
            ReceiverEmail = order.Receiver.Email;
            ReceiverPhone = order.Receiver.Phone;
        }

        public OrderViewModel() { }
    }
}
