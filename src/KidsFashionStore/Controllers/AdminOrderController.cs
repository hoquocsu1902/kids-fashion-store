using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using KidsFashionStore.Helpers;
using KidsFashionStore.Models.ViewModels.Admin;

namespace KidsFashionStore.Controllers
{
    /// <summary>Quản lý đơn hàng (F19): danh sách, chi tiết, cập nhật trạng thái.</summary>
    public class AdminOrderController : AdminBaseController
    {
        /// <summary>
        /// TRUE: khi admin HỦY đơn thì cộng trả tồn kho cho từng biến thể.
        /// Chỉ đúng nếu OrderController (Sử) đã TRỪ tồn kho lúc đặt hàng. Nếu Sử không trừ kho -> đổi thành false.
        /// </summary>
        private const bool RestoreStockOnCancel = true;

        // GET /AdminOrder?status=&keyword=&page=
        public ActionResult Index(string status, string keyword, int page = 1)
        {
            keyword = (keyword ?? "").Trim();
            var query = db.Orders.AsQueryable();

            if (!string.IsNullOrEmpty(status) && OrderStatus.All.Contains(status))
                query = query.Where(o => o.Status == status);
            else
                status = null;

            if (keyword.Length > 0)
            {
                int orderId;
                if (int.TryParse(keyword, out orderId))
                    query = query.Where(o => o.OrderId == orderId || o.ReceiverName.Contains(keyword) || o.Phone.Contains(keyword));
                else
                    query = query.Where(o => o.ReceiverName.Contains(keyword) || o.Phone.Contains(keyword));
            }

            var total = query.Count();
            var totalPages = TotalPagesFor(total, DefaultPageSize);
            page = Math.Min(Math.Max(page, 1), totalPages);

            var items = query
                .OrderByDescending(o => o.OrderDate).ThenByDescending(o => o.OrderId)
                .Skip((page - 1) * DefaultPageSize)
                .Take(DefaultPageSize)
                .Select(o => new OrderRowViewModel
                {
                    OrderId = o.OrderId,
                    OrderDate = o.OrderDate,
                    Username = o.User.Username,
                    ReceiverName = o.ReceiverName,
                    Phone = o.Phone,
                    TotalAmount = o.TotalAmount,
                    Status = o.Status
                }).ToList();

            var counts = db.Orders
                .GroupBy(o => o.Status)
                .Select(g => new { Status = g.Key, Total = g.Count() })
                .ToDictionary(x => x.Status, x => x.Total);

            return View(new OrderIndexViewModel
            {
                Items = items,
                Status = status,
                Keyword = keyword,
                Page = page,
                TotalPages = totalPages,
                TotalItems = total,
                StatusCounts = counts
            });
        }

        // GET /AdminOrder/Details/5
        public ActionResult Details(int? id)
        {
            if (!id.HasValue) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var o = db.Orders.AsNoTracking()
                .Where(x => x.OrderId == id.Value)
                .Select(x => new OrderDetailsViewModel
                {
                    OrderId = x.OrderId,
                    OrderDate = x.OrderDate,
                    Username = x.User.Username,
                    ReceiverName = x.ReceiverName,
                    Phone = x.Phone,
                    ShippingAddress = x.ShippingAddress,
                    Note = x.Note,
                    TotalAmount = x.TotalAmount,
                    Status = x.Status
                }).FirstOrDefault();
            if (o == null) return HttpNotFound();

            o.Lines = db.OrderDetails
                .Where(d => d.OrderId == id.Value)
                .OrderBy(d => d.OrderDetailId)
                .Select(d => new OrderLineViewModel
                {
                    ProductId = d.ProductVariant.ProductId,
                    ProductName = d.ProductVariant.Product.ProductName,
                    Size = d.ProductVariant.Size,
                    Color = d.ProductVariant.Color,
                    MainImage = d.ProductVariant.Product.MainImage,
                    Quantity = d.Quantity,
                    UnitPrice = d.UnitPrice
                }).ToList();
            o.AllowedNextStatuses = OrderStatus.NextOf(o.Status);
            return View(o);
        }

        // POST /AdminOrder/UpdateStatus
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateStatus(int orderId, string newStatus)
        {
            var order = db.Orders.Find(orderId);
            if (order == null) return HttpNotFound();

            if (string.IsNullOrEmpty(newStatus) || !OrderStatus.All.Contains(newStatus))
            {
                SetError("Trạng thái không hợp lệ.");
                return RedirectToAction("Details", new { id = orderId });
            }
            if (!OrderStatus.CanChange(order.Status, newStatus))
            {
                SetError("Không thể chuyển đơn từ \"" + OrderStatus.Label(order.Status) + "\" sang \"" + OrderStatus.Label(newStatus) + "\".");
                return RedirectToAction("Details", new { id = orderId });
            }

            if (newStatus == OrderStatus.Cancelled && RestoreStockOnCancel)
            {
                var lines = db.OrderDetails.Include(d => d.ProductVariant)
                    .Where(d => d.OrderId == orderId).ToList();
                foreach (var line in lines)
                    line.ProductVariant.StockQuantity += line.Quantity;
            }

            order.Status = newStatus;
            db.SaveChanges(); // đổi trạng thái + hoàn kho nằm trong cùng 1 lần lưu (atomic)

            SetSuccess("Đơn #" + orderId + " đã chuyển sang \"" + OrderStatus.Label(newStatus) + "\".");
            return RedirectToAction("Details", new { id = orderId });
        }
    }
}
