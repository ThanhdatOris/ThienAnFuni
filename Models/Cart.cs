namespace ThienAnFuni.Models
{
    public class Cart
    {
        public string Id { get; set; }
        public double TotalPrice { get; set; }
        public string CustomerId { get; set; }
        public Customer Customer { get; set; }
        public List<CartItem>? CartItems { get; set; }

        public void AddItem() { }
        public void RemoveItem() { }
        public void CalculateTotal() { }
        public void ClearCart() { }
    }
}
