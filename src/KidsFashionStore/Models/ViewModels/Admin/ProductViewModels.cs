using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;

namespace KidsFashionStore.Models.ViewModels.Admin
{
    public class ProductEditViewModel
    {
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên sản phẩm.")]
        [StringLength(200, ErrorMessage = "Tên sản phẩm tối đa 200 ký tự.")]
        [Display(Name = "Tên sản phẩm")]
        public string ProductName { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn danh mục.")]
        [Display(Name = "Danh mục")]
        public int? CategoryId { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập giá.")]
        [Range(0, 1000000000, ErrorMessage = "Giá phải từ 0 đến 1.000.000.000.")]
        [Display(Name = "Giá (VNĐ)")]
        public decimal? Price { get; set; }

        [Display(Name = "Mô tả")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn nhóm đối tượng.")]
        [RegularExpression("^(Boy|Girl|Unisex)$", ErrorMessage = "Nhóm đối tượng không hợp lệ.")]
        [Display(Name = "Dành cho")]
        public string Gender { get; set; }

        [Display(Name = "Đang bán (hiển thị)")]
        public bool IsActive { get; set; }

        /// <summary>Đường dẫn ảnh chính hiện tại (khi Edit).</summary>
        public string MainImage { get; set; }

        [Display(Name = "Ảnh chính")]
        public HttpPostedFileBase ImageFile { get; set; }

        public IEnumerable<SelectListItem> CategoryOptions { get; set; }
    }

    public class ProductRowViewModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string CategoryName { get; set; }
        public decimal Price { get; set; }
        public string MainImage { get; set; }
        public string Gender { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public int VariantCount { get; set; }
        public int TotalStock { get; set; }
    }

    public class ProductIndexViewModel
    {
        public List<ProductRowViewModel> Items { get; set; }
        public string Keyword { get; set; }
        public int? CategoryId { get; set; }
        public string Status { get; set; }
        public int Page { get; set; }
        public int TotalPages { get; set; }
        public int TotalItems { get; set; }
        public IEnumerable<SelectListItem> CategoryOptions { get; set; }
    }

    // ----- Trang quản lý biến thể + ảnh của 1 sản phẩm -----
    public class ProductManageViewModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string CategoryName { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public string MainImage { get; set; }
        public string Gender { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public List<VariantRowViewModel> Variants { get; set; }
        public List<ImageRowViewModel> Images { get; set; }
        public List<string> Promotions { get; set; }
    }

    public class VariantRowViewModel
    {
        public int VariantId { get; set; }
        public string Size { get; set; }
        public string Color { get; set; }
        public int StockQuantity { get; set; }
        public bool InOrders { get; set; }
    }

    public class ImageRowViewModel
    {
        public int ImageId { get; set; }
        public string ImageUrl { get; set; }
        public int SortOrder { get; set; }
        public bool IsMain { get; set; }
    }

    public class VariantEditViewModel
    {
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập size.")]
        [StringLength(10, ErrorMessage = "Size tối đa 10 ký tự.")]
        public string Size { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập màu.")]
        [StringLength(30, ErrorMessage = "Màu tối đa 30 ký tự.")]
        public string Color { get; set; }

        [Range(0, 100000, ErrorMessage = "Tồn kho phải từ 0 đến 100.000.")]
        public int StockQuantity { get; set; }
    }
}
