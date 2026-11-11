using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using KidsFashionStore.Helpers;
using KidsFashionStore.Models;
using KidsFashionStore.Models.ViewModels.Admin;

namespace KidsFashionStore.Controllers
{
    /// <summary>CRUD sản phẩm (F17) + quản lý biến thể size/màu/tồn kho và ảnh phụ (F18).</summary>
    public class AdminProductController : AdminBaseController
    {
        // GET /AdminProduct?keyword=&categoryId=&status=&page=
        public ActionResult Index(string keyword, int? categoryId, string status, int page = 1)
        {
            keyword = (keyword ?? "").Trim();
            var query = db.Products.AsQueryable();

            if (keyword.Length > 0)
                query = query.Where(p => p.ProductName.Contains(keyword));

            if (categoryId.HasValue)
            {
                var cid = categoryId.Value;
                // chọn danh mục cha -> lấy cả sản phẩm của các danh mục con
                var ids = db.Categories
                    .Where(c => c.CategoryId == cid || c.ParentId == cid)
                    .Select(c => c.CategoryId).ToList();
                query = query.Where(p => ids.Contains(p.CategoryId));
            }

            if (status == "active") query = query.Where(p => p.IsActive);
            else if (status == "hidden") query = query.Where(p => !p.IsActive);

            var total = query.Count();
            var totalPages = TotalPagesFor(total, DefaultPageSize);
            page = Math.Min(Math.Max(page, 1), totalPages);

            var items = query
                .OrderByDescending(p => p.ProductId)
                .Skip((page - 1) * DefaultPageSize)
                .Take(DefaultPageSize)
                .Select(p => new ProductRowViewModel
                {
                    ProductId = p.ProductId,
                    ProductName = p.ProductName,
                    CategoryName = p.Category.CategoryName,
                    Price = p.Price,
                    MainImage = p.MainImage,
                    Gender = p.Gender,
                    IsActive = p.IsActive,
                    CreatedDate = p.CreatedDate,
                    VariantCount = p.ProductVariants.Count(),
                    TotalStock = p.ProductVariants.Sum(v => (int?)v.StockQuantity) ?? 0
                }).ToList();

            return View(new ProductIndexViewModel
            {
                Items = items,
                Keyword = keyword,
                CategoryId = categoryId,
                Status = status,
                Page = page,
                TotalPages = totalPages,
                TotalItems = total,
                CategoryOptions = CategoryOptions(categoryId, false)
            });
        }

        // GET /AdminProduct/Details/5  (xem + quản lý biến thể, ảnh)
        public ActionResult Details(int? id)
        {
            if (!id.HasValue) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var model = BuildManageModel(id.Value);
            if (model == null) return HttpNotFound();
            return View(model);
        }

        // GET /AdminProduct/Create
        public ActionResult Create()
        {
            return View(new ProductEditViewModel
            {
                IsActive = true,
                Gender = "Unisex",
                CategoryOptions = CategoryOptions(null, true)
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ProductEditViewModel model)
        {
            ValidateProduct(model);

            // Tạo mới: bắt buộc có ảnh chính
            if (model.ImageFile == null || model.ImageFile.ContentLength == 0)
                ModelState.AddModelError("ImageFile", "Vui lòng chọn ảnh chính cho sản phẩm.");
            else
            {
                var err = ProductImageStorage.Validate(model.ImageFile);
                if (err != null) ModelState.AddModelError("ImageFile", err);
            }

            if (!ModelState.IsValid)
            {
                model.CategoryOptions = CategoryOptions(model.CategoryId, true);
                return View(model);
            }

            var path = ProductImageStorage.Save(model.ImageFile, Server, model.ProductName);
            var product = new Product
            {
                ProductName = model.ProductName.Trim(),
                CategoryId = model.CategoryId.Value,
                Price = model.Price.Value,
                Description = string.IsNullOrWhiteSpace(model.Description) ? null : model.Description.Trim(),
                MainImage = path,
                Gender = model.Gender,
                IsActive = model.IsActive,
                CreatedDate = DateTime.Now
            };
            // ảnh chính cũng nằm trong thư viện ảnh (giống dữ liệu seed)
            product.ProductImages.Add(new ProductImage { ImageUrl = path, SortOrder = 1 });
            db.Products.Add(product);
            db.SaveChanges();

            SetSuccess("Đã thêm sản phẩm. Hãy thêm biến thể (size/màu/tồn kho) để khách có thể mua.");
            return RedirectToAction("Details", new { id = product.ProductId });
        }

        // GET /AdminProduct/Edit/5
        public ActionResult Edit(int? id)
        {
            if (!id.HasValue) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var p = db.Products.Find(id.Value);
            if (p == null) return HttpNotFound();

            return View(new ProductEditViewModel
            {
                ProductId = p.ProductId,
                ProductName = p.ProductName,
                CategoryId = p.CategoryId,
                Price = p.Price,
                Description = p.Description,
                Gender = p.Gender ?? "Unisex",
                IsActive = p.IsActive,
                MainImage = p.MainImage,
                CategoryOptions = CategoryOptions(p.CategoryId, true)
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ProductEditViewModel model)
        {
            var p = db.Products.Find(model.ProductId);
            if (p == null) return HttpNotFound();

            ValidateProduct(model);
            var hasNewImage = model.ImageFile != null && model.ImageFile.ContentLength > 0;
            if (hasNewImage)
            {
                var err = ProductImageStorage.Validate(model.ImageFile);
                if (err != null) ModelState.AddModelError("ImageFile", err);
            }

            if (!ModelState.IsValid)
            {
                model.MainImage = p.MainImage;
                model.CategoryOptions = CategoryOptions(model.CategoryId, true);
                return View(model);
            }

            p.ProductName = model.ProductName.Trim();
            p.CategoryId = model.CategoryId.Value;
            p.Price = model.Price.Value;
            p.Description = string.IsNullOrWhiteSpace(model.Description) ? null : model.Description.Trim();
            p.Gender = model.Gender;
            p.IsActive = model.IsActive;

            if (hasNewImage)
            {
                var path = ProductImageStorage.Save(model.ImageFile, Server, model.ProductName);
                p.MainImage = path;
                var nextSort = (db.ProductImages.Where(i => i.ProductId == p.ProductId).Max(i => (int?)i.SortOrder) ?? 0) + 1;
                p.ProductImages.Add(new ProductImage { ImageUrl = path, SortOrder = nextSort });
            }

            db.SaveChanges();
            SetSuccess("Đã cập nhật sản phẩm \"" + p.ProductName + "\".");
            return RedirectToAction("Index");
        }

        // GET /AdminProduct/Delete/5
        public ActionResult Delete(int? id)
        {
            if (!id.HasValue) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var model = BuildManageModel(id.Value);
            if (model == null) return HttpNotFound();
            ViewBag.InOrders = db.OrderDetails.Any(d => d.ProductVariant.ProductId == id.Value);
            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var product = db.Products.Find(id);
            if (product == null) return HttpNotFound();

            // Sản phẩm đã có trong đơn cũ -> chỉ ẩn (soft delete) để không mất lịch sử đơn hàng.
            if (db.OrderDetails.Any(d => d.ProductVariant.ProductId == id))
            {
                product.IsActive = false;
                db.SaveChanges();
                SetSuccess("Sản phẩm \"" + product.ProductName + "\" đã có trong đơn hàng nên chỉ được ẨN (không xóa hẳn).");
                return RedirectToAction("Index");
            }

            db.Entry(product).Collection(p => p.Promotions).Load();
            product.Promotions.Clear();
            db.ProductImages.RemoveRange(db.ProductImages.Where(i => i.ProductId == id));
            db.ProductVariants.RemoveRange(db.ProductVariants.Where(v => v.ProductId == id));
            db.Products.Remove(product);
            db.SaveChanges();

            SetSuccess("Đã xóa sản phẩm \"" + product.ProductName + "\".");
            return RedirectToAction("Index");
        }

        // POST /AdminProduct/ToggleActive/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ToggleActive(int id)
        {
            var p = db.Products.Find(id);
            if (p == null) return HttpNotFound();
            p.IsActive = !p.IsActive;
            db.SaveChanges();
            SetSuccess((p.IsActive ? "Đã hiện" : "Đã ẩn") + " sản phẩm \"" + p.ProductName + "\".");
            return RedirectToAction("Index");
        }

        // ===================== BIẾN THỂ =====================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddVariant(VariantEditViewModel model)
        {
            if (!db.Products.Any(p => p.ProductId == model.ProductId)) return HttpNotFound();
            if (!ModelState.IsValid)
            {
                SetError(FirstError());
                return RedirectToAction("Details", new { id = model.ProductId });
            }

            var size = model.Size.Trim();
            var color = model.Color.Trim();
            var pid = model.ProductId;
            if (db.ProductVariants.Any(v => v.ProductId == pid && v.Size == size && v.Color == color))
            {
                SetError("Biến thể " + size + " / " + color + " đã tồn tại. Hãy chỉnh tồn kho của biến thể đó.");
                return RedirectToAction("Details", new { id = pid });
            }

            db.ProductVariants.Add(new ProductVariant
            {
                ProductId = pid,
                Size = size,
                Color = color,
                StockQuantity = model.StockQuantity
            });
            db.SaveChanges();
            SetSuccess("Đã thêm biến thể " + size + " / " + color + ".");
            return RedirectToAction("Details", new { id = pid });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateVariantStock(int variantId, int stockQuantity)
        {
            var v = db.ProductVariants.Find(variantId);
            if (v == null) return HttpNotFound();
            if (stockQuantity < 0 || stockQuantity > 100000)
            {
                SetError("Tồn kho phải từ 0 đến 100.000.");
                return RedirectToAction("Details", new { id = v.ProductId });
            }
            v.StockQuantity = stockQuantity;
            db.SaveChanges();
            SetSuccess("Đã cập nhật tồn kho " + v.Size + " / " + v.Color + " = " + stockQuantity + ".");
            return RedirectToAction("Details", new { id = v.ProductId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteVariant(int variantId)
        {
            var v = db.ProductVariants.Find(variantId);
            if (v == null) return HttpNotFound();
            var productId = v.ProductId;

            if (db.OrderDetails.Any(d => d.VariantId == variantId))
            {
                SetError("Biến thể đã có trong đơn hàng nên không thể xóa. Hãy đặt tồn kho = 0 để ngưng bán.");
                return RedirectToAction("Details", new { id = productId });
            }
            db.ProductVariants.Remove(v);
            db.SaveChanges();
            SetSuccess("Đã xóa biến thể " + v.Size + " / " + v.Color + ".");
            return RedirectToAction("Details", new { id = productId });
        }

        // ===================== ẢNH PHỤ =====================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddImage(int productId, HttpPostedFileBase imageFile)
        {
            var product = db.Products.Find(productId);
            if (product == null) return HttpNotFound();

            var err = ProductImageStorage.Validate(imageFile);
            if (err != null)
            {
                SetError(err);
                return RedirectToAction("Details", new { id = productId });
            }

            var path = ProductImageStorage.Save(imageFile, Server, product.ProductName);
            var nextSort = (db.ProductImages.Where(i => i.ProductId == productId).Max(i => (int?)i.SortOrder) ?? 0) + 1;
            db.ProductImages.Add(new ProductImage { ProductId = productId, ImageUrl = path, SortOrder = nextSort });
            if (string.IsNullOrEmpty(product.MainImage)) product.MainImage = path;
            db.SaveChanges();
            SetSuccess("Đã thêm ảnh.");
            return RedirectToAction("Details", new { id = productId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SetMainImage(int imageId)
        {
            var img = db.ProductImages.Find(imageId);
            if (img == null) return HttpNotFound();
            var product = db.Products.Find(img.ProductId);
            product.MainImage = img.ImageUrl;
            db.SaveChanges();
            SetSuccess("Đã đặt làm ảnh chính.");
            return RedirectToAction("Details", new { id = img.ProductId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteImage(int imageId)
        {
            var img = db.ProductImages.Find(imageId);
            if (img == null) return HttpNotFound();
            var productId = img.ProductId;
            var product = db.Products.Find(productId);

            if (product != null && product.MainImage == img.ImageUrl)
            {
                SetError("Đây đang là ảnh chính. Hãy đặt ảnh khác làm ảnh chính trước khi xóa.");
                return RedirectToAction("Details", new { id = productId });
            }
            // Chỉ xóa bản ghi DB; không xóa file vật lý để tránh làm hỏng ảnh dùng chung/seed.
            db.ProductImages.Remove(img);
            db.SaveChanges();
            SetSuccess("Đã xóa ảnh khỏi thư viện.");
            return RedirectToAction("Details", new { id = productId });
        }

        // ---------------- helpers ----------------
        private void ValidateProduct(ProductEditViewModel model)
        {
            if (model.CategoryId.HasValue)
            {
                var cid = model.CategoryId.Value;
                if (!db.Categories.Any(c => c.CategoryId == cid))
                    ModelState.AddModelError("CategoryId", "Danh mục không tồn tại.");
            }
            if (!string.IsNullOrWhiteSpace(model.ProductName))
            {
                var name = model.ProductName.Trim();
                var id = model.ProductId;
                if (db.Products.Any(p => p.ProductName == name && p.ProductId != id))
                    ModelState.AddModelError("ProductName", "Đã có sản phẩm cùng tên.");
            }
        }

        private string FirstError()
        {
            var e = ModelState.Values.SelectMany(v => v.Errors).FirstOrDefault();
            return e == null ? "Dữ liệu không hợp lệ." : e.ErrorMessage;
        }

        private ProductManageViewModel BuildManageModel(int id)
        {
            var p = db.Products.AsNoTracking().FirstOrDefault(x => x.ProductId == id);
            if (p == null) return null;

            return new ProductManageViewModel
            {
                ProductId = p.ProductId,
                ProductName = p.ProductName,
                CategoryName = db.Categories.Where(c => c.CategoryId == p.CategoryId).Select(c => c.CategoryName).FirstOrDefault(),
                Price = p.Price,
                Description = p.Description,
                MainImage = p.MainImage,
                Gender = p.Gender,
                IsActive = p.IsActive,
                CreatedDate = p.CreatedDate,
                Variants = db.ProductVariants
                    .Where(v => v.ProductId == id)
                    .OrderBy(v => v.Size).ThenBy(v => v.Color)
                    .Select(v => new VariantRowViewModel
                    {
                        VariantId = v.VariantId,
                        Size = v.Size,
                        Color = v.Color,
                        StockQuantity = v.StockQuantity,
                        InOrders = v.OrderDetails.Any()
                    }).ToList(),
                Images = db.ProductImages
                    .Where(i => i.ProductId == id)
                    .OrderBy(i => i.SortOrder)
                    .Select(i => new ImageRowViewModel
                    {
                        ImageId = i.ImageId,
                        ImageUrl = i.ImageUrl,
                        SortOrder = i.SortOrder,
                        IsMain = i.ImageUrl == p.MainImage
                    }).ToList(),
                Promotions = db.Promotions
                    .Where(pr => pr.Products.Any(x => x.ProductId == id))
                    .Select(pr => new { pr.PromotionName, pr.DiscountPercent })
                    .ToList()
                    .Select(pr => pr.PromotionName + " (-" + pr.DiscountPercent + "%)")
                    .ToList()
            };
        }
    }
}
