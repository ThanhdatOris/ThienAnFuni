namespace ThienAnFuni.Models
{
    public class Order
    {
        public string Id { get; set; }
        public string CustomerPhoneNumber { get; set; }
        public double TotalPrice { get; set; }
        public DateTime OrderDate { get; set; }
        public int OrderStatus { get; set; }
        public string PaymentStatus { get; set; }
        public string PaymentMethod { get; set; }
        public string InvoiceNumber { get; set; }
        public DateTime InvoiceDate { get; set; }

        public string CustomerId { get; set; }
        public Customer Customer { get; set; }
        public string? SaleStaffId { get; set; }
        public SaleStaff? SaleStaff { get; set; }
        public List<OrderItem>? OrderItems { get; set; }
    }
}
