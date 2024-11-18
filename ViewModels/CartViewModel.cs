using ThienAnFuni.Models;

namespace ThienAnFuni.ViewModels
{
    public class CartViewModel
    {
        public Dictionary<int, CartDetail> CartItems { get; set; }
        public decimal Total { get; set; } // Tổng giá đã bao gồm VAT
    }
}
