namespace ThienAnFuni.Models
{
    public class SaleStaff : User
    {
        public string CitizenId { get; set; }
        public DateTime IssuingDate { get; set; }
        public string IssuingPlace { get; set; }

        public void AssistCustomerOrder() { }
        public List<Order>? Orders { get; set; }
    }
}
