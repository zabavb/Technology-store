using Library.Models;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Client.Models.Orders
{
    public class OrderViewModel
    {
        public long Id { get; set; }
        public List<Product> Items { get; set; } = new();

        [Display(Name = "Items ids (id,id,id,...)")]
        [RegularExpression(@"^(\d+)(,\d+)*$", ErrorMessage = "The Items ids must be in the format: id,id,id with at least one id")]
        public string ItemsIds { get; set; } = string.Empty;

        [Display(Name = "Receiver username")]
        public string ReceiverUsername { get; set; } = string.Empty;

        [Display(Name = "Receiver name")]
        public string ReceiverName { get; set; } = string.Empty;

        [Display(Name = "Receiver email")]
        public string ReceiverEmail { get; set; } = string.Empty;

        [Display(Name = "Receiver phone")]
        public string ReceiverPhone { get; set; } = string.Empty;

        [Required(ErrorMessage = "Country is required")]
        [StringLength(32, MinimumLength = 2, ErrorMessage = "Country must be in range between 2 and 32 characters")]
        public string Country { get; set; } = string.Empty;

        [Required(ErrorMessage = "Locality is required")]
        [StringLength(32, MinimumLength = 2, ErrorMessage = "Locality must be in range between 2 and 32 characters")]
        public string Locality { get; set; } = string.Empty;
        
        [Display(Name = "Receiver address")]
        [Required(ErrorMessage = "Address of receiver is required")]
        [StringLength(32, MinimumLength = 2, ErrorMessage = "Address must be in range between 2 and 32 characters")]
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

        public OrderViewModel(Order order, List<Product> items, User receiver)
        {
            Id = order.Id;
            Items = items;
            ItemsIds = string.Join(",", order.ItemsIds);
            ReceiverUsername = receiver.Username;
            ReceiverName = receiver.FirstName + receiver.LastName;
            ReceiverEmail = receiver.Email;
            ReceiverPhone = receiver.Phone;
            Country = order.Country;
            Locality = order.Locality;
            Address = order.Address;
        }

        public OrderViewModel() { }
    }
}
