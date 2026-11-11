using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using KidsFashionStore.Models;

namespace KidsFashionStore.Controllers
{
    /// <summary>
    /// Lớp cha của mọi controller quản trị. Tự kiểm tra Session["Role"] == "Admin" trước MỌI action
    /// (F22): chưa đăng nhập -> chuyển về Login; đã đăng nhập nhưng không phải Admin -> 403.
    /// </summary>
    public abstract class AdminBaseController : Controller
    {
        protected readonly QL_ThoiTrangTreEmEntities db = new QL_ThoiTrangTreEmEntities();

        protected const int DefaultPageSize = 10;

        protected bool IsAdmin()
        {
            return Session["Role"] != null && Session["Role"].ToString() == "Admin";
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!IsAdmin())
            {
                if (Session["Role"] == null)
                {
                    filterContext.Result = RedirectToAction("Login", "Account",
                        new { returnUrl = Request.RawUrl });
                }
                else
                {
                    filterContext.Result = new HttpStatusCodeResult(
                        HttpStatusCode.Forbidden, "Bạn không có quyền truy cập khu vực quản trị.");
                }
                return;
            }
            base.OnActionExecuting(filterContext);
        }

        /// <summary>
        /// Lỗi ghi DB (khóa ngoại, trùng khóa duy nhất, ...) -> báo lỗi thân thiện và quay lại trang trước,
        /// thay vì hiện trang lỗi vàng của ASP.NET. Các lỗi khác vẫn đi đường mặc định.
        /// </summary>
        protected override void OnException(ExceptionContext filterContext)
        {
            var message = DbErrorMessage(filterContext.Exception);
            if (message == null)
            {
                base.OnException(filterContext);
                return;
            }

            SetError(message);
            filterContext.ExceptionHandled = true;

            var referrer = Request.UrlReferrer;
            if (referrer != null && Url.IsLocalUrl(referrer.PathAndQuery))
                filterContext.Result = Redirect(referrer.PathAndQuery);
            else
                filterContext.Result = RedirectToAction("Index");
        }

        private static string DbErrorMessage(Exception ex)
        {
            if (ex is DbEntityValidationException)
                return "Dữ liệu không hợp lệ, vui lòng kiểm tra lại.";
            if (!(ex is DbUpdateException))
                return null;

            for (var e = ex; e != null; e = e.InnerException)
            {
                var sql = e as SqlException;
                if (sql == null) continue;
                if (sql.Number == 547)
                    return "Không thể lưu: dữ liệu đang được liên kết với bản ghi khác hoặc vi phạm ràng buộc.";
                if (sql.Number == 2627 || sql.Number == 2601)
                    return "Không thể lưu: dữ liệu bị trùng.";
            }
            return "Không thể lưu dữ liệu. Vui lòng thử lại.";
        }

        protected void SetSuccess(string message) { TempData["Success"] = message; }
        protected void SetError(string message) { TempData["Error"] = message; }

        /// <summary>Danh sách danh mục dạng "Cha › Con" cho dropdown.</summary>
        protected List<SelectListItem> CategoryOptions(int? selectedId, bool onlyActive)
        {
            var all = db.Categories.AsNoTracking().ToList();
            var items = new List<SelectListItem>();
            foreach (var parent in all.Where(c => c.ParentId == null).OrderBy(c => c.CategoryName))
            {
                if (!onlyActive || parent.IsActive || parent.CategoryId == selectedId)
                {
                    items.Add(new SelectListItem
                    {
                        Value = parent.CategoryId.ToString(),
                        Text = parent.CategoryName,
                        Selected = parent.CategoryId == selectedId
                    });
                }
                var parentId = parent.CategoryId;
                foreach (var child in all.Where(c => c.ParentId == parentId).OrderBy(c => c.CategoryName))
                {
                    if (onlyActive && !child.IsActive && child.CategoryId != selectedId) continue;
                    items.Add(new SelectListItem
                    {
                        Value = child.CategoryId.ToString(),
                        Text = parent.CategoryName + " › " + child.CategoryName,
                        Selected = child.CategoryId == selectedId
                    });
                }
            }
            return items;
        }

        protected static int TotalPagesFor(int totalItems, int pageSize)
        {
            return System.Math.Max(1, (int)System.Math.Ceiling(totalItems / (double)pageSize));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}
