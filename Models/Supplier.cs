using System.ComponentModel.DataAnnotations;

namespace ThienAnFuni.Models
{
    public class Supplier
    {
        [Display(Name = "Mã NCC")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên hiển thị không được để trống")]
        [StringLength(200, ErrorMessage = "Tên hiển thị không vượt quá 200 kí tự")]
        [Display(Name = "Tên công ty cung cấp")]
        public string DisplayName { get; set; }

        [Required(ErrorMessage = "Địa chỉ không được để trống")]
        [Display(Name = "Địa chỉ")]
        public string Address { get; set; }

        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [Display(Name = "Email")]
        public string? Email { get; set; }

        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        [Display(Name = "Số điện thoại")]
        public string PhoneNumber { get; set; }

        public string? Image { get; set; }

        [Required]
        [Display(Name = "Trạng thái hoạt động")]
        public bool IsActive { get; set; }

        // Quan hệ với Shipment
        public virtual ICollection<Shipment>? Shipments { get; set; }
    }
}
