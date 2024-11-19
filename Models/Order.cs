using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ThienAnFuni.Models
{
    public class Order
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Số điện thoại khách hàng bắt buộc phải có.")]
        public string CustomerPhoneNumber { get; set; } = string.Empty;
        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Tổng tiền phải là số")]
        public double TotalPrice { get; set; }
        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Số lượng phải là số")]
        [Display(Name = "Tổng số lượng")]
        public int TotalQuantity { get; set; }
        [Display(Name = "Ghi chú")]
        public string? Note { get; set; }
        [Display(Name = "Địa chỉ")]
        public string Address { get; set; } = string.Empty;
        [Display(Name = "Ngày đặt hàng")]
        public DateTime OrderDate { get; set; } = DateTime.Now;
        [Display(Name = "Trạng thái")]
        public int? OrderStatus { get; set; }
        [Display(Name = "Phương thức thanh toán")]
        public int? PaymentMethod { get; set; }
        [Display(Name = "Trạng thái thanh toán")]
        public int? PaymentStatus { get; set; }
        [Display(Name = "Số hóa đơn")]
        public string? InvoiceNumber { get; set; }
        [Display(Name = "Ngày xuất hóa đơn")]
        public DateTime? InvoiceDate { get; set; }

        public string? CustomerId { get; set; }
        public virtual Customer? Customer { get; set; }

        public string? SaleStaffId { get; set; }
        public virtual SaleStaff? SaleStaff { get; set; }
        public string? ManagerId { get; set; }
        public virtual Manager? Manager { get; set; }
        public virtual ICollection<OrderDetail>? OrderDetails { get; set; }
    }
}
