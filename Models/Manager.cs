namespace ThienAnFuni.Models
{
    public class Manager : User
    {
        public string CitizenId { get; set; }

        public void ChangeInfo() { }
        public virtual List<Shipment>? Shipments { get; set; }
        public virtual List<Category>? Categories { get; set; }
    }
}
