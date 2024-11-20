using System.ComponentModel.DataAnnotations;

namespace ThienAnFuni.Models
{
    public class CartDetail
    {
        public int Id { get; set; }
        [Required]
        [Display(Name = "Số lượng")]
        public int Quantity { get; set; }
        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Giá phải là số")]
        [Display(Name = "Giá")]
        public double Price { get; set; }
        [Display(Name = "Mã sản phẩm")]
        public int ProductId { get; set; }
        public virtual Product? Product { get; set; }
        [Display(Name = "Mã giỏ hàng")]
        public int CartId { get; set; }
        public virtual Cart? Cart { get; set; }
    }
}
