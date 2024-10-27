namespace ThienAnFuni.Models
{
    public class OrderItem
    {
        public string Id { get; set; }
        public string ProductName { get; set; }
        public double PriceAtOrder { get; set; }
        public double Subtotal { get; set; }
        public int Quantity { get; set; }

        public string OrderId { get; set; }
        public Order Order { get; set; }
        public string ProductId { get; set; }
        public Product Product { get; set; }
    }
}
