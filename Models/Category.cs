using ThienAnFuni.Models;
using System.ComponentModel.DataAnnotations;

public class Category
{
    public int Id { get; set; }
    public int? ParentId { get; set; }

    [Required(ErrorMessage = "Tên danh mục là bắt buộc.")]
    [StringLength(100, ErrorMessage = "Tên danh mục không được vượt quá 100 ký tự.")]
    public required string Name { get; set; }

    public string? RoomType { get; set; } // 
    public string? UsageType { get; set; } // 
    public string? Description { get; set; } // 
    public bool IsActive { get; set; } = true;
    // Navigation parent cate and product 
    public virtual Category? ParentCategory { get; set; }
    public virtual List<Category>? SubCategories { get; set; }
    public virtual List<Product>? Products { get; set; }

}
