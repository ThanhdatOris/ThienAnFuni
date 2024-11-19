using System.ComponentModel.DataAnnotations;

namespace ThienAnFuni.Models
{
    public class OrderDetail
    {
        [Display(Name = "Mã HĐ")]
        public int Id { get; set; }
        [Display(Name = "Số hóa đơn")]
        public double PriceAtOrder { get; set; }
        [Display(Name = "Số lượng")]
        public int Quantity { get; set; }

        public int OrderId { get; set; }
        public Order? Order { get; set; }
        public int ProductId { get; set; }
        public Product? Product { get; set; }
    }
}
