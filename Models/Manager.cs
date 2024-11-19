using System.ComponentModel.DataAnnotations;

namespace ThienAnFuni.Models
{
    public class Manager : User
    {
        [Display(Name = "CCCD")]
        public string CitizenId { get; set; } = string.Empty;
        [Display(Name = "Ngày cấp")]
        public DateTime IssuingDate { get; set; }
        [Display(Name = "Nơi cấp")]
        public string IssuingPlace { get; set; } = string.Empty;

        public DateTime StartDate { get; set; } = DateTime.Now;
        //[Display(Name = "Ngày kết thúc hợp đồng")]
        public DateTime? EndDate { get; set; }
        [Display(Name = "Trình độ")]
        public string? Degree { get; set; } = string.Empty;

        public void ChangeInfo() { }
        public virtual ICollection<Order>? Orders { get; set; }
        public virtual List<Shipment>? Shipments { get; set; }
        public virtual List<Category>? Categories { get; set; }
    }
}
