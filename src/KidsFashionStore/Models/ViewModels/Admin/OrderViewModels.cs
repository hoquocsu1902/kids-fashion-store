using System;
using System.Collections.Generic;

namespace KidsFashionStore.Models.ViewModels.Admin
{
    public class OrderRowViewModel
    {
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public string Username { get; set; }
        public string ReceiverName { get; set; }
        public string Phone { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }
    }

    public class OrderIndexViewModel
    {
        public List<OrderRowViewModel> Items { get; set; }
        public string Status { get; set; }
        public string Keyword { get; set; }
        public int Page { get; set; }
        public int TotalPages { get; set; }
        public int TotalItems { get; set; }
        public Dictionary<string, int> StatusCounts { get; set; }
    }

    public class OrderLineViewModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string Size { get; set; }
        public string Color { get; set; }
        public string MainImage { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal LineTotal { get { return Quantity * UnitPrice; } }
    }

    public class OrderDetailsViewModel
    {
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public string Username { get; set; }
        public string ReceiverName { get; set; }
        public string Phone { get; set; }
        public string ShippingAddress { get; set; }
        public string Note { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }
        public List<OrderLineViewModel> Lines { get; set; }
        public string[] AllowedNextStatuses { get; set; }
    }
}
