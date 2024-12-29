using ThienAnFuni.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ThienAnFuni.Models
{
    public class Category
    {
        public int Id { get; set; }
        [Display(Name = "Danh mục cha")]
        public int? ParentId { get; set; }

        [Required(ErrorMessage = "Tên danh mục là bắt buộc.")]
        [StringLength(100, ErrorMessage = "Tên danh mục không được vượt quá 100 ký tự.")]
        [Display(Name = "Tên danh mục")]
        public string Name { get; set; }

        [Display(Name = "Ảnh danh mục")]
        public string? Image { get; set; }

        public string Slug { get; set; }

        [Display(Name = "Trạng thái hoạt động")]
        public bool IsActive { get; set; }

        // "Navigation" ParentId của cate và product 
        [ForeignKey("ParentId")]
        public virtual Category? ParentCategory { get; set; }
        public virtual List<Category>? SubCategories { get; set; }
        public virtual List<Product>? Products { get; set; }

    }
}