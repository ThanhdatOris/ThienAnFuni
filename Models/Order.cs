﻿using System.ComponentModel.DataAnnotations;

namespace ThienAnFuni.Models
{
    public class Order
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Số điện thoại khách hàng bắt buộc phải có.")]
        public string CustomerPhoneNumber { get; set; }
        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Tổng tiền phải là số")]
        public double TotalPrice { get; set; }
        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Số lượng phải là số")]
        public int TotalQuantity { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.Now;
        public int OrderStatus { get; set; }
        public string PaymentStatus { get; set; }
        public string PaymentMethod { get; set; }
        public string InvoiceNumber { get; set; }
        public DateTime InvoiceDate { get; set; }

        public int? CustomerId { get; set; }
        public virtual Customer? Customer { get; set; }
        public int? SaleStaffId { get; set; }
        public virtual SaleStaff? SaleStaff { get; set; }
        public virtual ICollection<OrderDetail>? OrderDetails { get; set; }
    }
}
