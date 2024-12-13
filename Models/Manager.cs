using System.ComponentModel.DataAnnotations;

namespace ThienAnFuni.Models
{
    public class Manager : User
    {
        //[Display(Name = "CCCD")]
        //public string CitizenId { get; set; }
        //[Display(Name = "Ngày cấp")]
        //public DateTime IssuingDate { get; set; }
        //[Display(Name = "Nơi cấp")]
        //public string? IssuingPlace { get; set; }
        //[Display(Name = "Ngày bắt đầu làm việc")]
        //public DateTime StartDate { get; set; }
        //[Display(Name = "Ngày nghỉ việc")]
        //public DateTime? EndDate { get; set; }
        //[Display(Name = "Trình độ")]
        //public string? Degree { get; set; } 

        public virtual ICollection<Order>? Orders { get; set; }
        public virtual List<Shipment>? Shipments { get; set; }
        public virtual List<Category>? Categories { get; set; }
    }
}
