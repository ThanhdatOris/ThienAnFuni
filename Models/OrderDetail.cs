namespace ThienAnFuni.Models
{
    public class OrderDetail
    {
        public int Id { get; set; }
        public string ProductName { get; set; }
        public double PriceAtOrder { get; set; }
        public double Subtotal { get; set; }
        public int Quantity { get; set; }

        public int OrderId { get; set; }
        public Order? Order { get; set; }
        public int ProductId { get; set; }
        public Product? Product { get; set; }
    }
}
