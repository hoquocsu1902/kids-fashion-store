using System.Linq;
using System.Web.Mvc;
using KidsFashionStore.Helpers;
using KidsFashionStore.Models.ViewModels.Admin;

namespace KidsFashionStore.Controllers
{
    /// <summary>Dashboard quản trị: GET /Admin</summary>
    public class AdminController : AdminBaseController
    {
        public ActionResult Index()
        {
            var statusCounts = db.Orders
                .GroupBy(o => o.Status)
                .Select(g => new { Status = g.Key, Total = g.Count() })
                .ToDictionary(x => x.Status, x => x.Total);

            var model = new DashboardViewModel
            {
                TotalProducts = db.Products.Count(),
                ActiveProducts = db.Products.Count(p => p.IsActive),
                TotalCategories = db.Categories.Count(),
                TotalCustomers = db.Users.Count(u => u.Role == "Customer"),
                TotalOrders = db.Orders.Count(),
                PendingOrders = db.Orders.Count(o => o.Status == OrderStatus.Pending),
                CompletedRevenue = db.Orders
                    .Where(o => o.Status == OrderStatus.Completed)
                    .Sum(o => (decimal?)o.TotalAmount) ?? 0m,
                OrdersByStatus = statusCounts,
                RecentOrders = db.Orders
                    .OrderByDescending(o => o.OrderDate)
                    .Take(5)
                    .Select(o => new OrderRowViewModel
                    {
                        OrderId = o.OrderId,
                        OrderDate = o.OrderDate,
                        Username = o.User.Username,
                        ReceiverName = o.ReceiverName,
                        Phone = o.Phone,
                        TotalAmount = o.TotalAmount,
                        Status = o.Status
                    }).ToList(),
                LowStock = db.ProductVariants
                    .Where(v => v.StockQuantity <= 5 && v.Product.IsActive)
                    .OrderBy(v => v.StockQuantity)
                    .Take(8)
                    .Select(v => new LowStockViewModel
                    {
                        ProductId = v.ProductId,
                        ProductName = v.Product.ProductName,
                        Size = v.Size,
                        Color = v.Color,
                        StockQuantity = v.StockQuantity
                    }).ToList()
            };
            return View(model);
        }
    }
}
