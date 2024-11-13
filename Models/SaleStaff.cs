namespace ThienAnFuni.Models
{
    public class SaleStaff : User
    {
        public string CitizenId { get; set; } = string.Empty;
        public DateTime IssuingDate { get; set; }
        public string IssuingPlace { get; set; } = string.Empty;
        public DateTime StartDate { get; set; } = DateTime.Now;
        public DateTime? EndDate { get; set; }
        public string? Degree { get; set; } = string.Empty;

        public virtual ICollection<Order>? Orders { get; set; }
        public void AssistCustomerOrder() { }
    }
}
