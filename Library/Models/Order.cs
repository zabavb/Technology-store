﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Models
{
    public class Order
    {
        public long Id { get; set; }
        public Product Item { get; set; } = new();
        public User Receiver { get; set; } = new();
        public string Country { get; set; } = string.Empty;
        public string Locality { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public DateOnly DeliveryDate { get; set; } = DateOnly.FromDateTime(DateTime.Now);

        public Order(long id, Product item, User receiver, string country, string locality, string address, DateOnly deliveryDate)
        {
            Id = id;
            Item = item;
            Receiver = receiver;
            Country = country;
            Locality = locality;
            Address = address;
            DeliveryDate = deliveryDate;
        }

        public Order() { }
    }
}