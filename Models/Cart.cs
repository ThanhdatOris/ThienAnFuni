using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ThienAnFuni.Models
{
    public class Cart
    {
        [Display(Name = "Mã")]
        public int Id { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Tổng tiền phải là số và không được để trống")]
        [Display(Name = "Tổng tiền")]
        public double TotalPrice { get; set; }
        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Số lượng phải là số và không được để trống")]
        [Display(Name = "Số lượng")]
        public int TotalQuantity { get; set; }

        [ForeignKey("Customer")]  // Chỉ định Cart.CustomerId là khóa ngoại tham chiếu đến Customer

        public string CustomerId { get; set; }
        public virtual Customer? Customer { get; set; }
        public virtual ICollection<CartDetail>? CartDetails { get; set; }

        public void AddItem() { }
        public void RemoveItem() { }
        public void CalculateTotal() { }
        public void ClearCart() { }
    }
}
