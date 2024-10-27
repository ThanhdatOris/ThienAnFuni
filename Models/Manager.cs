namespace ThienAnFuni.Models
{
    public class Manager : User
    {
        public string CitizenId { get; set; }

        public void ChangeInfo() { }
        public List<Shipment> Shipments { get; set; }
        public List<Category> Categories { get; set; }
    }
}
