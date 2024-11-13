using System.ComponentModel.DataAnnotations.Schema;

namespace ThienAnFuni.Models
{
    public class Customer : User
    {
        public virtual List<Order>? Orders { get; set; }
        [InverseProperty("Customer")]
        public virtual Cart? Cart { get; set; }
        public void ChangeInfo() { }
        public void ViewOrderHistory() { }
    }
}
