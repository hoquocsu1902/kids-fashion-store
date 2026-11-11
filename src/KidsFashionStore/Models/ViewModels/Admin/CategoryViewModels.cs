using System.ComponentModel.DataAnnotations;

namespace KidsFashionStore.Models.ViewModels.Admin
{
    public class CategoryEditViewModel
    {
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên danh mục.")]
        [StringLength(100, ErrorMessage = "Tên danh mục tối đa 100 ký tự.")]
        [Display(Name = "Tên danh mục")]
        public string CategoryName { get; set; }

        [Display(Name = "Danh mục cha")]
        public int? ParentId { get; set; }

        [StringLength(255, ErrorMessage = "Mô tả tối đa 255 ký tự.")]
        [Display(Name = "Mô tả")]
        public string Description { get; set; }

        [Display(Name = "Đang hiển thị")]
        public bool IsActive { get; set; }
    }

    public class CategoryRowViewModel
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int? ParentId { get; set; }
        public string ParentName { get; set; }
        public int Level { get; set; }          // 0 = cha, 1 = con
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public int ChildCount { get; set; }
        public int ProductCount { get; set; }
    }
}
