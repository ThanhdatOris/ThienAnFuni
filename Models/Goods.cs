namespace ThienAnFuni.Models
{
    public class Goods
    {
        public string Id { get; set; }
        public int Quantity { get; set; }
        public double ImportPrice { get; set; }
        public double TotalPrice { get; set; }
        public string ShipmentId { get; set; }
        public Shipment Shipment { get; set; }
        public string ProductId { get; set; }
        public Product Product { get; set; }
    }
}
