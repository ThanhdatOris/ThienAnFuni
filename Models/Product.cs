using System.ComponentModel.DataAnnotations;

namespace ThienAnFuni.Models
{
    public class Product
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Tên sản phẩm không được để trống")]
        [StringLength(200)]
        public string Name { get; set; }

        [Required(ErrorMessage = "Giá không được để trống")]
        [Range(0, double.MaxValue, ErrorMessage = "Giá là số")]
        public double Price { get; set; }

        public string? Unit { get; set; }

        public string? Material { get; set; }

        public string? Dimension { get; set; }

        public string? Standard { get; set; }

        public string? Color { get; set; }

        public string? Type { get; set; }

        public string? Brand { get; set; }

        public string? WarrantyPeriod { get; set; }

        public string? Description { get; set; }

        [Required]
        public bool Active { get; set; } = true;
        public string? MainImg { get; set; }
        public int? CategoryId { get; set; }
        public Category? Category { get; set; }
        public List<OrderItem>? OrderItems { get; set; }
        public List<CartItem>? CartItems { get; set; }
        public List<Goods>? Goods { get; set; }
    }
}
