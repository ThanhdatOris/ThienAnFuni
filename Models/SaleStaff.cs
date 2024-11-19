using System.ComponentModel.DataAnnotations;

namespace ThienAnFuni.Models
{
    public class SaleStaff : User
    {
        [Display(Name = "CCCD")]
        public string CitizenId { get; set; } = string.Empty;
        [Display(Name = "Ngày cấp")]
        public DateTime IssuingDate { get; set; }
        [Display(Name = "Nơi cấp")]
        public string IssuingPlace { get; set; } = string.Empty;
        [Display(Name = "Ngày bắt đầu làm việc")]
        public DateTime StartDate { get; set; } = DateTime.Now;
        public DateTime? EndDate { get; set; }
        [Display(Name = "Trình độ")] 
        public string? Degree { get; set; } = string.Empty;

        public virtual ICollection<Order>? Orders { get; set; }
        public void AssistCustomerOrder() { }
    }
}
