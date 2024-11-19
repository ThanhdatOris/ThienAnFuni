using System.ComponentModel.DataAnnotations;

namespace ThienAnFuni.Models
{
    public class Product
    {
        [Display(Name = "Mã sản phẩm")]
        public int Id { get; set; }
        [Required(ErrorMessage = "Tên sản phẩm không được để trống")]
        [StringLength(200)]
        [Display(Name = "Tên sản phẩm")]
        public string Name { get; set; }
        [Display(Name = "Giá")]
        public double Price { get; set; }
        [Display(Name = "Danh mục")]
        public string? Unit { get; set; }
        [Display(Name = "Chất liệu")]
        public string? Material { get; set; }
        [Display(Name = "Kích thướt")]
        public string? Dimension { get; set; }
        [Display(Name = "Tiêu chuẩn")]
        public string? Standard { get; set; }
        [Display(Name = "Màu sắc")]
        public string? Color { get; set; }
        [Display(Name = "Thương hiệu")]
        public string? Brand { get; set; }
        [Display(Name = "Thời gian bảo hành")]
        public string? WarrantyPeriod { get; set; }
        [Display(Name = "Mô tả")]
        public string? Description { get; set; }
        [Display(Name = "Hình ảnh")]
        public string? MainImg { get; set; }
        [Display(Name = "Ngày tạo")]
        public DateOnly? CreatedDate { get; set; }

        [Required]
        [Display(Name = "Trạng thái")]
        public bool IsActive { get; set; } = true;
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
