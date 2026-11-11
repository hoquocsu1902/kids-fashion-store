using System;
using System.Collections.Generic;

namespace KidsFashionStore.Models.ViewModels.Admin
{
    public class RevenueViewModel
    {
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public decimal TotalRevenue { get; set; }
        public int CompletedOrders { get; set; }
        public decimal AverageOrderValue { get; set; }
        public List<RevenuePeriodViewModel> Monthly { get; set; }
        public List<TopProductViewModel> TopProducts { get; set; }
        public Dictionary<string, int> OrdersByStatus { get; set; }
    }

    public class RevenuePeriodViewModel
    {
        public string Label { get; set; }
        public decimal Revenue { get; set; }
        public int OrderCount { get; set; }
    }

    public class TopProductViewModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal Revenue { get; set; }
    }
}
