using ThienAnFuni.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ThienAnFuni.Models
{
    public class Category
    {
        public int Id { get; set; }
        public int? ParentId { get; set; }

        [Required(ErrorMessage = "Tên danh mục là bắt buộc.")]
        [StringLength(100, ErrorMessage = "Tên danh mục không được vượt quá 100 ký tự.")]
        public string Name { get; set; }
        public bool IsActive { get; set; }
        // Navigation parent cate and product 
        [ForeignKey("ParentId")]
        public virtual Category? ParentCategory { get; set; }
        public virtual List<Category>? SubCategories { get; set; }
        public virtual List<Product>? Products { get; set; }

    }
}