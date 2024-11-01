using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Models
{
    public class Order
    {
        public long Id { get; set; }
        public User Receiver { get; set; } = new();
        public Product Product { get; set; } = new();
        public DateTime Date { get; set; } = DateTime.Now;
        public string Payment { get; set; } = PaymentType.None.ToString();
    }
}
