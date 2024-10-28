using System.ComponentModel.DataAnnotations;

namespace ThienAnFuni.Models
{
    public class Supplier
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên hiển thị không được để trống")]
        [StringLength(200, ErrorMessage = "Tên hiển thị không vượt quá 200 kí tự")]
        public string DisplayName { get; set; }

        [Required(ErrorMessage = "Địa chỉ không được để trống")]
        public string Address { get; set; }

        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string? Email { get; set; }

        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        public string PhoneNumber { get; set; }

        public string? Image { get; set; }

        [Required]
        public bool IsActive { get; set; }

        // Quan hệ với Shipment
        public virtual ICollection<Shipment>? Shipments { get; set; }
    }
}
