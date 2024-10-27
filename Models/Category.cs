using ThienAnFuni.Models;
using System.ComponentModel.DataAnnotations;

public class Category
{
    public int Id { get; set; }
    public int? ParentId { get; set; } // Change this to string?

    [Required(ErrorMessage = "Tên danh mục là bắt buộc.")]
    [StringLength(100, ErrorMessage = "Tên danh mục không được vượt quá 100 ký tự.")]
    public required string Name { get; set; }

    public string? RoomType { get; set; }
    public string? UsageType { get; set; }
    public string? Description { get; set; }
    public bool Active { get; set; } = true;
    public List<Product>? Products { get; set; }
}
