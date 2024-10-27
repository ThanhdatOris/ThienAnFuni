namespace ThienAnFuni.Models
{
    public class CartItem
    {
        public string Id { get; set; }
        public int Quantity { get; set; }
        public double Price { get; set; }
        public string ProductId { get; set; }
        public Product Product { get; set; }
        public string CartId { get; set; }
        public Cart Cart { get; set; }
    }
}
