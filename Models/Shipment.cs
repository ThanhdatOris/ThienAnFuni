namespace ThienAnFuni.Models
{
    public class Shipment
    {
        public string Id { get; set; }
        public DateTime ReceiptDate { get; set; }
        public double TotalPrice { get; set; }
        public double TotalQuantity { get; set; }
        public string Note { get; set; }

        public string SupplierId { get; set; }
        public Supplier Supplier { get; set; }
        public string ManagerId { get; set; }
        public Manager Manager { get; set; }
        public List<Goods> Goods { get; set; }
    }
}
