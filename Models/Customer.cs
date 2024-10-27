namespace ThienAnFuni.Models
{
    public class Customer : User
    {
        public void ChangeInfo() { }
        public void ViewOrderHistory() { }
        public List<Order> Orders { get; set; }
        public Cart Cart { get; set; }
    }
}
