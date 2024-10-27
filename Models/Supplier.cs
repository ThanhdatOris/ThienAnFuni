namespace ThienAnFuni.Models
{
    public class Supplier
    {
        public string Id { get; set; }
        public string DisplayName { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string? Image { get; set; }
        public bool Active { get; set; }
        public List<Shipment>? Shipments { get; set; }
    }
}
