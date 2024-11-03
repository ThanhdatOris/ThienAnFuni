using System.ComponentModel.DataAnnotations;

namespace ThienAnFuni.Models
{
    public class Product
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Tên sản phẩm không được để trống")]
        [StringLength(200)]
        public string Name { get; set; }

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

        public string? MainImg { get; set; }

        [Required]
        public bool IsActive { get; set; } = false;
        //[Required]
        public bool? IsImport { get; set; } = false;
        public int? CategoryId { get; set; }
        public virtual Category? Category { get; set; }
        public virtual ICollection<OrderDetail>? OrderItems { get; set; }
        public virtual ICollection<CartDetail>? CartItems { get; set; }
        public virtual ICollection<Goods>? Goods { get; set; }
        public virtual ICollection<ProductImage>? ProductImages { get; set; }

    }
}
