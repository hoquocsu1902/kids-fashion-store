using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using KidsFashionStore.Models;
using KidsFashionStore.Models.ViewModels.Admin;

namespace KidsFashionStore.Controllers
{
    /// <summary>
    /// CRUD khuyến mãi (F20). Quan hệ N-N với Product qua bảng ProductPromotions
    /// (EF tự sinh navigation Promotion.Products / Product.Promotions).
    /// </summary>
    public class PromotionController : AdminBaseController
    {
        // GET /Promotion
        public ActionResult Index()
        {
            var today = DateTime.Today;
            var rows = db.Promotions
                .OrderByDescending(p => p.StartDate)
                .Select(p => new
                {
                    p.PromotionId,
                    p.PromotionName,
                    p.DiscountPercent,
                    p.StartDate,
                    p.EndDate,
                    p.IsActive,
                    ProductCount = p.Products.Count()
                }).ToList()
                .Select(p =>
                {
                    string label, css;
                    if (!p.IsActive) { label = "Đã tắt"; css = "badge-cancelled"; }
                    else if (today < p.StartDate) { label = "Sắp diễn ra"; css = "badge-pending"; }
                    else if (today > p.EndDate) { label = "Đã hết hạn"; css = "badge-none"; }
                    else { label = "Đang chạy"; css = "badge-completed"; }

                    return new PromotionRowViewModel
                    {
                        PromotionId = p.PromotionId,
                        PromotionName = p.PromotionName,
                        DiscountPercent = p.DiscountPercent,
                        StartDate = p.StartDate,
                        EndDate = p.EndDate,
                        IsActive = p.IsActive,
                        ProductCount = p.ProductCount,
                        StatusLabel = label,
                        StatusCss = css
                    };
                }).ToList();
            return View(rows);
        }

        // GET /Promotion/Create
        public ActionResult Create()
        {
            var model = new PromotionEditViewModel
            {
                IsActive = true,
                DiscountPercent = 10,
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddDays(30),
                SelectedProductIds = new List<int>()
            };
            FillProducts(model);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(PromotionEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                FillProducts(model);
                return View(model);
            }

            var promo = new Promotion
            {
                PromotionName = model.PromotionName.Trim(),
                DiscountPercent = model.DiscountPercent.Value,
                StartDate = model.StartDate.Value.Date,
                EndDate = model.EndDate.Value.Date,
                IsActive = model.IsActive
            };
            var ids = (model.SelectedProductIds ?? new List<int>()).Distinct().ToList();
            foreach (var product in db.Products.Where(p => ids.Contains(p.ProductId)).ToList())
                promo.Products.Add(product);

            db.Promotions.Add(promo);
            db.SaveChanges();
            SetSuccess("Đã thêm khuyến mãi \"" + promo.PromotionName + "\".");
            return RedirectToAction("Index");
        }

        // GET /Promotion/Edit/5
        public ActionResult Edit(int? id)
        {
            if (!id.HasValue) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var promo = db.Promotions.Include(p => p.Products).FirstOrDefault(p => p.PromotionId == id.Value);
            if (promo == null) return HttpNotFound();

            var model = new PromotionEditViewModel
            {
                PromotionId = promo.PromotionId,
                PromotionName = promo.PromotionName,
                DiscountPercent = promo.DiscountPercent,
                StartDate = promo.StartDate,
                EndDate = promo.EndDate,
                IsActive = promo.IsActive,
                SelectedProductIds = promo.Products.Select(p => p.ProductId).ToList()
            };
            FillProducts(model);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(PromotionEditViewModel model)
        {
            var promo = db.Promotions.Include(p => p.Products).FirstOrDefault(p => p.PromotionId == model.PromotionId);
            if (promo == null) return HttpNotFound();

            if (!ModelState.IsValid)
            {
                FillProducts(model);
                return View(model);
            }

            promo.PromotionName = model.PromotionName.Trim();
            promo.DiscountPercent = model.DiscountPercent.Value;
            promo.StartDate = model.StartDate.Value.Date;
            promo.EndDate = model.EndDate.Value.Date;
            promo.IsActive = model.IsActive;

            // đồng bộ danh sách sản phẩm áp dụng
            var selected = (model.SelectedProductIds ?? new List<int>()).Distinct().ToList();
            foreach (var p in promo.Products.Where(p => !selected.Contains(p.ProductId)).ToList())
                promo.Products.Remove(p);
            var existing = promo.Products.Select(p => p.ProductId).ToList();
            var toAdd = selected.Except(existing).ToList();
            foreach (var p in db.Products.Where(p => toAdd.Contains(p.ProductId)).ToList())
                promo.Products.Add(p);

            db.SaveChanges();
            SetSuccess("Đã cập nhật khuyến mãi \"" + promo.PromotionName + "\".");
            return RedirectToAction("Index");
        }

        // GET /Promotion/Delete/5
        public ActionResult Delete(int? id)
        {
            if (!id.HasValue) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var promo = db.Promotions.Include(p => p.Products).AsNoTracking().FirstOrDefault(p => p.PromotionId == id.Value);
            if (promo == null) return HttpNotFound();
            ViewBag.ProductCount = promo.Products.Count;
            return View(new PromotionRowViewModel
            {
                PromotionId = promo.PromotionId,
                PromotionName = promo.PromotionName,
                DiscountPercent = promo.DiscountPercent,
                StartDate = promo.StartDate,
                EndDate = promo.EndDate,
                IsActive = promo.IsActive,
                ProductCount = promo.Products.Count
            });
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var promo = db.Promotions.Include(p => p.Products).FirstOrDefault(p => p.PromotionId == id);
            if (promo == null) return HttpNotFound();

            promo.Products.Clear(); // xóa các dòng trong ProductPromotions
            db.Promotions.Remove(promo);
            db.SaveChanges();
            SetSuccess("Đã xóa khuyến mãi \"" + promo.PromotionName + "\".");
            return RedirectToAction("Index");
        }

        // POST /Promotion/ToggleActive/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ToggleActive(int id)
        {
            var promo = db.Promotions.Find(id);
            if (promo == null) return HttpNotFound();
            promo.IsActive = !promo.IsActive;
            db.SaveChanges();
            SetSuccess((promo.IsActive ? "Đã bật" : "Đã tắt") + " khuyến mãi \"" + promo.PromotionName + "\".");
            return RedirectToAction("Index");
        }

        private void FillProducts(PromotionEditViewModel model)
        {
            var selected = new HashSet<int>(model.SelectedProductIds ?? new List<int>());
            model.AllProducts = db.Products.AsNoTracking()
                .OrderBy(p => p.Category.CategoryName).ThenBy(p => p.ProductName)
                .Select(p => new ProductCheckItem
                {
                    ProductId = p.ProductId,
                    ProductName = p.ProductName,
                    CategoryName = p.Category.CategoryName
                }).ToList();
            foreach (var item in model.AllProducts)
                item.Checked = selected.Contains(item.ProductId);
        }
    }
}
