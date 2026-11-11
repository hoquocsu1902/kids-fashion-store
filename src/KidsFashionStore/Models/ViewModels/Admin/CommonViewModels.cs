using System.Collections.Generic;
using System.Web.Routing;

namespace KidsFashionStore.Models.ViewModels.Admin
{
    public class PagerModel
    {
        public int Page { get; set; }
        public int TotalPages { get; set; }
        public string Action { get; set; }
        public RouteValueDictionary RouteValues { get; set; }

        public PagerModel(int page, int totalPages, string action, RouteValueDictionary routeValues)
        {
            Page = page;
            TotalPages = totalPages;
            Action = action;
            RouteValues = routeValues ?? new RouteValueDictionary();
        }
    }

    public class DashboardViewModel
    {
        public int TotalProducts { get; set; }
        public int ActiveProducts { get; set; }
        public int TotalCategories { get; set; }
        public int TotalCustomers { get; set; }
        public int TotalOrders { get; set; }
        public int PendingOrders { get; set; }
        public decimal CompletedRevenue { get; set; }
        public Dictionary<string, int> OrdersByStatus { get; set; }
        public List<OrderRowViewModel> RecentOrders { get; set; }
        public List<LowStockViewModel> LowStock { get; set; }
    }

    public class LowStockViewModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string Size { get; set; }
        public string Color { get; set; }
        public int StockQuantity { get; set; }
    }
}
