using System.ComponentModel.DataAnnotations;

namespace ThienAnFuni.Models
{
    public class Cart
    {
        public int Id { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Tổng tiền phải là số và không được để trống")]
        public double TotalPrice { get; set; }
        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Số lượng phải là số và không được để trống")]
        public int TotalQuantity { get; set; }
        public int CustomerId { get; set; }
        public virtual Customer? Customer { get; set; }
        public virtual ICollection<CartDetail>? CartDetails { get; set; }

        public void AddItem() { }
        public void RemoveItem() { }
        public void CalculateTotal() { }
        public void ClearCart() { }
    }
}
