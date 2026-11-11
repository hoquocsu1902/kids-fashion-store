using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Web.Http;
using KidsFashionStore.Filters;
using KidsFashionStore.Models;

namespace KidsFashionStore.Controllers
{
    // ===== DTO: KHÔNG trả trực tiếp entity EF (tránh lỗi vòng lặp navigation khi serialize JSON/XML) =====
    public class VariantDto
    {
        public int VariantId { get; set; }
        public string Size { get; set; }
        public string Color { get; set; }
        public int StockQuantity { get; set; }
    }

    public class ProductDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public string MainImage { get; set; }
        public string Gender { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public List<VariantDto> Variants { get; set; }   // chỉ có khi GET /api/products/{id}
    }

    public class ProductInputModel
    {
        [Required(ErrorMessage = "ProductName là bắt buộc.")]
        [StringLength(200)]
        public string ProductName { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "CategoryId không hợp lệ.")]
        public int CategoryId { get; set; }

        [Range(0, 1000000000, ErrorMessage = "Price phải từ 0 đến 1.000.000.000.")]
        public decimal Price { get; set; }

        public string Description { get; set; }

        [StringLength(255)]
        public string MainImage { get; set; }

        [RegularExpression("^(Boy|Girl|Unisex)$", ErrorMessage = "Gender chỉ nhận Boy, Girl hoặc Unisex.")]
        public string Gender { get; set; }

        public bool IsActive { get; set; }
    }

    public class ApiMessage
    {
        public string Message { get; set; }
    }

    /// <summary>
    /// Web API sản phẩm (F23) - dùng chung EF context với MVC (cùng database QL_ThoiTrangTreEm).
    /// Cần WebApiConfig.Register gọi config.MapHttpAttributeRoutes() (template MVC + Web API đã có sẵn).
    /// </summary>
    [RoutePrefix("api/products")]
    public class ProductApiController : ApiController
    {
        private readonly QL_ThoiTrangTreEmEntities db = new QL_ThoiTrangTreEmEntities();

        // GET api/products?keyword=ao&categoryId=3&includeInactive=false
        [HttpGet, Route("")]
        public IHttpActionResult GetProducts(string keyword = null, int? categoryId = null, bool includeInactive = false)
        {
            var query = db.Products.AsQueryable();
            if (!includeInactive) query = query.Where(p => p.IsActive);
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var k = keyword.Trim();
                query = query.Where(p => p.ProductName.Contains(k));
            }
            if (categoryId.HasValue)
            {
                var cid = categoryId.Value;
                query = query.Where(p => p.CategoryId == cid);
            }

            var list = query.OrderBy(p => p.ProductId).Select(p => new ProductDto
            {
                ProductId = p.ProductId,
                ProductName = p.ProductName,
                CategoryId = p.CategoryId,
                CategoryName = p.Category.CategoryName,
                Price = p.Price,
                Description = p.Description,
                MainImage = p.MainImage,
                Gender = p.Gender,
                IsActive = p.IsActive,
                CreatedDate = p.CreatedDate
            }).ToList();
            return Ok(list);
        }

        // GET api/products/5  -> 200 hoặc 404
        [HttpGet, Route("{id:int}", Name = "GetProductById")]
        public IHttpActionResult GetProduct(int id)
        {
            var dto = db.Products.Where(p => p.ProductId == id).Select(p => new ProductDto
            {
                ProductId = p.ProductId,
                ProductName = p.ProductName,
                CategoryId = p.CategoryId,
                CategoryName = p.Category.CategoryName,
                Price = p.Price,
                Description = p.Description,
                MainImage = p.MainImage,
                Gender = p.Gender,
                IsActive = p.IsActive,
                CreatedDate = p.CreatedDate
            }).FirstOrDefault();
            if (dto == null) return NotFound();

            dto.Variants = db.ProductVariants.Where(v => v.ProductId == id)
                .OrderBy(v => v.Size).ThenBy(v => v.Color)
                .Select(v => new VariantDto { VariantId = v.VariantId, Size = v.Size, Color = v.Color, StockQuantity = v.StockQuantity })
                .ToList();
            return Ok(dto);
        }

        // POST api/products  -> 201 Created hoặc 400
        [HttpPost, Route(""), ApiKeyRequired]
        public IHttpActionResult PostProduct([FromBody] ProductInputModel model)
        {
            if (model == null) return BadRequest("Body rỗng hoặc sai định dạng JSON.");
            ValidateInput(model);
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var product = new Product
            {
                ProductName = model.ProductName.Trim(),
                CategoryId = model.CategoryId,
                Price = model.Price,
                Description = model.Description,
                MainImage = model.MainImage,
                Gender = model.Gender,
                IsActive = model.IsActive,
                CreatedDate = DateTime.Now
            };
            db.Products.Add(product);
            db.SaveChanges();

            var dto = ToDto(product);
            return CreatedAtRoute("GetProductById", new { id = product.ProductId }, dto);
        }

        // PUT api/products/5  -> 200 hoặc 400/404
        [HttpPut, Route("{id:int}"), ApiKeyRequired]
        public IHttpActionResult PutProduct(int id, [FromBody] ProductInputModel model)
        {
            if (model == null) return BadRequest("Body rỗng hoặc sai định dạng JSON.");
            var product = db.Products.Find(id);
            if (product == null) return NotFound();

            ValidateInput(model, id);
            if (!ModelState.IsValid) return BadRequest(ModelState);

            product.ProductName = model.ProductName.Trim();
            product.CategoryId = model.CategoryId;
            product.Price = model.Price;
            product.Description = model.Description;
            product.MainImage = model.MainImage;
            product.Gender = model.Gender;
            product.IsActive = model.IsActive;
            db.SaveChanges();
            return Ok(ToDto(product));
        }

        // DELETE api/products/5  -> 200 hoặc 404. Đã có trong đơn hàng => chỉ ẩn (IsActive=false)
        [HttpDelete, Route("{id:int}"), ApiKeyRequired]
        public IHttpActionResult DeleteProduct(int id)
        {
            var product = db.Products.Find(id);
            if (product == null) return NotFound();

            if (db.OrderDetails.Any(d => d.ProductVariant.ProductId == id))
            {
                product.IsActive = false;
                db.SaveChanges();
                return Ok(new ApiMessage { Message = "Sản phẩm đã có trong đơn hàng nên chỉ bị ẩn (IsActive=false)." });
            }

            db.Entry(product).Collection(p => p.Promotions).Load();
            product.Promotions.Clear();
            db.ProductImages.RemoveRange(db.ProductImages.Where(i => i.ProductId == id));
            db.ProductVariants.RemoveRange(db.ProductVariants.Where(v => v.ProductId == id));
            db.Products.Remove(product);
            db.SaveChanges();
            return Ok(new ApiMessage { Message = "Đã xóa sản phẩm." });
        }

        // ---------------- helpers ----------------
        private void ValidateInput(ProductInputModel model, int? currentId = null)
        {
            if (!string.IsNullOrWhiteSpace(model.ProductName))
            {
                var name = model.ProductName.Trim();
                var id = currentId ?? 0;
                if (db.Products.Any(p => p.ProductName == name && p.ProductId != id))
                    ModelState.AddModelError("ProductName", "Đã có sản phẩm cùng tên.");
            }
            var cid = model.CategoryId;
            if (cid > 0 && !db.Categories.Any(c => c.CategoryId == cid))
                ModelState.AddModelError("CategoryId", "Danh mục không tồn tại.");
        }

        private ProductDto ToDto(Product p)
        {
            return new ProductDto
            {
                ProductId = p.ProductId,
                ProductName = p.ProductName,
                CategoryId = p.CategoryId,
                CategoryName = db.Categories.Where(c => c.CategoryId == p.CategoryId).Select(c => c.CategoryName).FirstOrDefault(),
                Price = p.Price,
                Description = p.Description,
                MainImage = p.MainImage,
                Gender = p.Gender,
                IsActive = p.IsActive,
                CreatedDate = p.CreatedDate
            };
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}
