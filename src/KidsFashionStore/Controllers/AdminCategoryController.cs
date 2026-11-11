using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using KidsFashionStore.Models;
using KidsFashionStore.Models.ViewModels.Admin;

namespace KidsFashionStore.Controllers
{
    /// <summary>CRUD danh mục 2 cấp (F16). Danh mục cha: ParentId = null; danh mục con: ParentId = Id của cha.</summary>
    public class AdminCategoryController : AdminBaseController
    {
        // GET /AdminCategory
        public ActionResult Index()
        {
            var all = db.Categories.AsNoTracking().ToList();
            var productCounts = db.Products
                .GroupBy(p => p.CategoryId)
                .Select(g => new { CategoryId = g.Key, Total = g.Count() })
                .ToDictionary(x => x.CategoryId, x => x.Total);

            var rows = new List<CategoryRowViewModel>();
            foreach (var parent in all.Where(c => c.ParentId == null).OrderBy(c => c.CategoryName))
            {
                rows.Add(ToRow(parent, 0, null, all, productCounts));
                var parentId = parent.CategoryId;
                foreach (var child in all.Where(c => c.ParentId == parentId).OrderBy(c => c.CategoryName))
                    rows.Add(ToRow(child, 1, parent.CategoryName, all, productCounts));
            }
            return View(rows);
        }

        // GET /AdminCategory/Details/5
        public ActionResult Details(int? id)
        {
            if (!id.HasValue) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var category = db.Categories.AsNoTracking().FirstOrDefault(c => c.CategoryId == id.Value);
            if (category == null) return HttpNotFound();

            var all = db.Categories.AsNoTracking().ToList();
            var counts = db.Products
                .GroupBy(p => p.CategoryId)
                .Select(g => new { CategoryId = g.Key, Total = g.Count() })
                .ToDictionary(x => x.CategoryId, x => x.Total);

            string parentName = null;
            if (category.ParentId.HasValue)
                parentName = all.Where(c => c.CategoryId == category.ParentId.Value).Select(c => c.CategoryName).FirstOrDefault();

            ViewBag.Children = all.Where(c => c.ParentId == category.CategoryId)
                .OrderBy(c => c.CategoryName)
                .Select(c => ToRow(c, 1, category.CategoryName, all, counts)).ToList();
            return View(ToRow(category, category.ParentId.HasValue ? 1 : 0, parentName, all, counts));
        }

        // GET /AdminCategory/Create
        public ActionResult Create(int? parentId)
        {
            var model = new CategoryEditViewModel { IsActive = true, ParentId = parentId };
            FillParentList(model.ParentId, 0);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CategoryEditViewModel model)
        {
            ValidateCategory(model);
            if (!ModelState.IsValid)
            {
                FillParentList(model.ParentId, 0);
                return View(model);
            }

            db.Categories.Add(new Category
            {
                CategoryName = model.CategoryName.Trim(),
                ParentId = model.ParentId,
                Description = string.IsNullOrWhiteSpace(model.Description) ? null : model.Description.Trim(),
                IsActive = model.IsActive
            });
            db.SaveChanges();
            SetSuccess("Đã thêm danh mục \"" + model.CategoryName.Trim() + "\".");
            return RedirectToAction("Index");
        }

        // GET /AdminCategory/Edit/5
        public ActionResult Edit(int? id)
        {
            if (!id.HasValue) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var c = db.Categories.Find(id.Value);
            if (c == null) return HttpNotFound();

            var model = new CategoryEditViewModel
            {
                CategoryId = c.CategoryId,
                CategoryName = c.CategoryName,
                ParentId = c.ParentId,
                Description = c.Description,
                IsActive = c.IsActive
            };
            FillParentList(model.ParentId, c.CategoryId);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(CategoryEditViewModel model)
        {
            var c = db.Categories.Find(model.CategoryId);
            if (c == null) return HttpNotFound();

            ValidateCategory(model);
            if (!ModelState.IsValid)
            {
                FillParentList(model.ParentId, model.CategoryId);
                return View(model);
            }

            c.CategoryName = model.CategoryName.Trim();
            c.ParentId = model.ParentId;
            c.Description = string.IsNullOrWhiteSpace(model.Description) ? null : model.Description.Trim();
            c.IsActive = model.IsActive;
            db.SaveChanges();
            SetSuccess("Đã cập nhật danh mục \"" + c.CategoryName + "\".");
            return RedirectToAction("Index");
        }

        // GET /AdminCategory/Delete/5  (trang xác nhận)
        public ActionResult Delete(int? id)
        {
            if (!id.HasValue) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var c = db.Categories.AsNoTracking().FirstOrDefault(x => x.CategoryId == id.Value);
            if (c == null) return HttpNotFound();

            var all = db.Categories.AsNoTracking().ToList();
            var counts = db.Products
                .GroupBy(p => p.CategoryId)
                .Select(g => new { CategoryId = g.Key, Total = g.Count() })
                .ToDictionary(x => x.CategoryId, x => x.Total);
            return View(ToRow(c, c.ParentId.HasValue ? 1 : 0, null, all, counts));
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var c = db.Categories.Find(id);
            if (c == null) return HttpNotFound();

            // Ràng buộc trước khi xóa (F16)
            if (db.Categories.Any(x => x.ParentId == id))
            {
                SetError("Không thể xóa: danh mục đang có danh mục con. Hãy xóa/chuyển danh mục con trước.");
                return RedirectToAction("Index");
            }
            if (db.Products.Any(p => p.CategoryId == id))
            {
                SetError("Không thể xóa: danh mục đang có sản phẩm. Hãy chuyển sản phẩm sang danh mục khác, hoặc dùng \"Ẩn\" để tạm ngưng hiển thị.");
                return RedirectToAction("Index");
            }

            db.Categories.Remove(c);
            db.SaveChanges();
            SetSuccess("Đã xóa danh mục \"" + c.CategoryName + "\".");
            return RedirectToAction("Index");
        }

        // POST /AdminCategory/ToggleActive/5  (ẩn/hiện nhanh thay cho xóa)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ToggleActive(int id)
        {
            var c = db.Categories.Find(id);
            if (c == null) return HttpNotFound();
            c.IsActive = !c.IsActive;
            db.SaveChanges();
            SetSuccess((c.IsActive ? "Đã hiện" : "Đã ẩn") + " danh mục \"" + c.CategoryName + "\".");
            return RedirectToAction("Index");
        }

        // ---------------- helpers ----------------
        private void ValidateCategory(CategoryEditViewModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.CategoryName))
            {
                var name = model.CategoryName.Trim();
                var parentId = model.ParentId;
                var id = model.CategoryId;
                if (db.Categories.Any(c => c.CategoryName == name && c.ParentId == parentId && c.CategoryId != id))
                    ModelState.AddModelError("CategoryName", "Đã có danh mục cùng tên trong cùng cấp này.");
            }

            if (model.ParentId.HasValue)
            {
                var parentId = model.ParentId.Value;
                var parent = db.Categories.AsNoTracking().FirstOrDefault(c => c.CategoryId == parentId);
                if (parent == null)
                    ModelState.AddModelError("ParentId", "Danh mục cha không tồn tại.");
                else if (parent.ParentId != null)
                    ModelState.AddModelError("ParentId", "Chỉ hỗ trợ menu 2 cấp: danh mục cha phải là danh mục cấp 1.");
                else if (model.CategoryId > 0 && parent.CategoryId == model.CategoryId)
                    ModelState.AddModelError("ParentId", "Danh mục không thể là cha của chính nó.");
                else if (model.CategoryId > 0 && db.Categories.Any(c => c.ParentId == model.CategoryId))
                    ModelState.AddModelError("ParentId", "Danh mục này đang có danh mục con nên không thể chuyển thành danh mục con.");
            }
        }

        // Chỉ cho chọn danh mục cấp 1 làm cha, loại chính nó.
        private void FillParentList(int? selectedId, int excludeId)
        {
            var parents = db.Categories.AsNoTracking()
                .Where(c => c.ParentId == null && c.CategoryId != excludeId)
                .OrderBy(c => c.CategoryName)
                .ToList();
            ViewBag.ParentList = new SelectList(parents, "CategoryId", "CategoryName", selectedId);
        }

        private static CategoryRowViewModel ToRow(Category c, int level, string parentName,
            List<Category> all, Dictionary<int, int> productCounts)
        {
            int count;
            productCounts.TryGetValue(c.CategoryId, out count);
            return new CategoryRowViewModel
            {
                CategoryId = c.CategoryId,
                CategoryName = c.CategoryName,
                ParentId = c.ParentId,
                ParentName = parentName,
                Level = level,
                Description = c.Description,
                IsActive = c.IsActive,
                ChildCount = all.Count(x => x.ParentId == c.CategoryId),
                ProductCount = count
            };
        }
    }
}
