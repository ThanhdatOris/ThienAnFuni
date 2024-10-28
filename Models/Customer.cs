namespace ThienAnFuni.Models
{
    public class Customer : User
    {
        public virtual List<Order>? Orders { get; set; }

        public virtual Cart? Cart { get; set; }
        public void ChangeInfo() { }
        public void ViewOrderHistory() { }
    }
}
