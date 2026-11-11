using System;
using System.Linq;
using System.Web.Mvc;
using KidsFashionStore.Helpers;
using KidsFashionStore.Models.ViewModels.Admin;

namespace KidsFashionStore.Controllers
{
    /// <summary>Thống kê doanh thu (F21): CHỈ tính các đơn có Status = Completed.</summary>
    public class ReportController : AdminBaseController
    {
        // GET /Report/Revenue?from=2026-09-01&to=2026-10-31
        public ActionResult Revenue(DateTime? from, DateTime? to)
        {
            if (from.HasValue && to.HasValue && from.Value > to.Value)
            {
                SetError("Ngày bắt đầu phải nhỏ hơn hoặc bằng ngày kết thúc. Đã bỏ qua bộ lọc ngày.");
                from = null;
                to = null;
            }

            DateTime? fromDate = from.HasValue ? from.Value.Date : (DateTime?)null;
            DateTime? toExclusive = to.HasValue ? to.Value.Date.AddDays(1) : (DateTime?)null; // gồm cả ngày "đến"

            var inRange = db.Orders.AsQueryable();
            if (fromDate.HasValue) inRange = inRange.Where(o => o.OrderDate >= fromDate.Value);
            if (toExclusive.HasValue) inRange = inRange.Where(o => o.OrderDate < toExclusive.Value);

            var completed = inRange.Where(o => o.Status == OrderStatus.Completed);

            var orders = completed.Select(o => new { o.OrderDate, o.TotalAmount }).ToList();
            var totalRevenue = orders.Sum(o => o.TotalAmount);
            var count = orders.Count;

            var monthly = orders
                .GroupBy(o => new { o.OrderDate.Year, o.OrderDate.Month })
                .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month)
                .Select(g => new RevenuePeriodViewModel
                {
                    Label = g.Key.Month.ToString("00") + "/" + g.Key.Year,
                    Revenue = g.Sum(x => x.TotalAmount),
                    OrderCount = g.Count()
                }).ToList();

            var top = db.OrderDetails
                .Where(d => d.Order.Status == OrderStatus.Completed
                            && (!fromDate.HasValue || d.Order.OrderDate >= fromDate.Value)
                            && (!toExclusive.HasValue || d.Order.OrderDate < toExclusive.Value))
                .GroupBy(d => new { d.ProductVariant.ProductId, d.ProductVariant.Product.ProductName })
                .Select(g => new TopProductViewModel
                {
                    ProductId = g.Key.ProductId,
                    ProductName = g.Key.ProductName,
                    Quantity = g.Sum(x => x.Quantity),
                    Revenue = g.Sum(x => x.Quantity * x.UnitPrice)
                })
                .OrderByDescending(x => x.Revenue)
                .Take(5)
                .ToList();

            var byStatus = inRange
                .GroupBy(o => o.Status)
                .Select(g => new { Status = g.Key, Total = g.Count() })
                .ToDictionary(x => x.Status, x => x.Total);

            return View(new RevenueViewModel
            {
                From = from,
                To = to,
                TotalRevenue = totalRevenue,
                CompletedOrders = count,
                AverageOrderValue = count == 0 ? 0m : Math.Round(totalRevenue / count, 0),
                Monthly = monthly,
                TopProducts = top,
                OrdersByStatus = byStatus
            });
        }
    }
}
