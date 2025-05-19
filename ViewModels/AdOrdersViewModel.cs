using ThienAnFuni.Models;

namespace ThienAnFuni.ViewModels
{
    public class AdOrdersViewModel
    {
        public List<Order> PendingOrders { get; set; } = new List<Order>();
        public List<Order> SuccessOrders { get; set; } = new List<Order>();
    }
}
