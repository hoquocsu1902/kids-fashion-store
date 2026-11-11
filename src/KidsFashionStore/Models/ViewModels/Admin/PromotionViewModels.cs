using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace KidsFashionStore.Models.ViewModels.Admin
{
    public class PromotionEditViewModel : IValidatableObject
    {
        public int PromotionId { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên khuyến mãi.")]
        [StringLength(150, ErrorMessage = "Tên khuyến mãi tối đa 150 ký tự.")]
        [Display(Name = "Tên khuyến mãi")]
        public string PromotionName { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập % giảm.")]
        [Range(0, 100, ErrorMessage = "% giảm phải từ 0 đến 100.")]
        [Display(Name = "% giảm giá")]
        public int? DiscountPercent { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn ngày bắt đầu.")]
        [DataType(DataType.Date)]
        [Display(Name = "Ngày bắt đầu")]
        public DateTime? StartDate { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn ngày kết thúc.")]
        [DataType(DataType.Date)]
        [Display(Name = "Ngày kết thúc")]
        public DateTime? EndDate { get; set; }

        [Display(Name = "Đang bật")]
        public bool IsActive { get; set; }

        /// <summary>Các ProductId được áp dụng (checkbox name="SelectedProductIds").</summary>
        public List<int> SelectedProductIds { get; set; }

        public List<ProductCheckItem> AllProducts { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (StartDate.HasValue && EndDate.HasValue && StartDate.Value > EndDate.Value)
            {
                yield return new ValidationResult(
                    "Ngày kết thúc phải lớn hơn hoặc bằng ngày bắt đầu.", new[] { "EndDate" });
            }
        }
    }

    public class ProductCheckItem
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string CategoryName { get; set; }
        public bool Checked { get; set; }
    }

    public class PromotionRowViewModel
    {
        public int PromotionId { get; set; }
        public string PromotionName { get; set; }
        public int DiscountPercent { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }
        public int ProductCount { get; set; }
        public string StatusLabel { get; set; }
        public string StatusCss { get; set; }
    }
}
