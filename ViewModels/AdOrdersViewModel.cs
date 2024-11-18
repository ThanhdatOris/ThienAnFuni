using ThienAnFuni.Models;

namespace ThienAnFuni.ViewModels
{
    public class AdOrdersViewModel
    {
        public IEnumerable<Order> PendingOrders { get; set; } = new List<Order>();
        public IEnumerable<Order> SuccessOrders { get; set; } = new List<Order>();
    }
}
