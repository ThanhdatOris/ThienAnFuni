using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace ThienAnFuni.Models
{
    public class User : IdentityUser
    {
        //public int Id { get; set; }

        [Required(ErrorMessage = "Tên không được để trống")]
        [StringLength(100, ErrorMessage = "Tên không thể dài hơn 100 kí tự")]
        [Display(Name = "Họ và tên")]
        public string? FullName { get; set; }
        //[Display(Name = "Tên")]
        //public string Username { get; set; }

        [Required(ErrorMessage = "Số điện thoại không được để trống")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        [Display(Name ="Số điện thoại")]
        public override string? PhoneNumber { get; set; }

        [Display(Name = "Địa chỉ")]
        public string? Address { get; set; }
        [Display(Name = "Giới tính")]
        public string? Gender { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Ngày sinh")]
        public DateTime? DateOfBirth { get; set; }

        [Required(ErrorMessage = "Password bắt buộc nhập")]
        [DataType(DataType.Password)]
        //public string Password { get; set; }
        [Display(Name = "Hoạt động")]
        public bool IsActive { get; set; }

        [Display(Name = "CCCD")]
        public string? CitizenId { get; set; }

        [Display(Name = "Ngày cấp")]
        public DateTime? IssuingDate { get; set; }

        [Display(Name = "Nơi cấp")]
        public string? IssuingPlace { get; set; }

        [Display(Name = "Ngày bắt đầu làm việc")]
        public DateTime? StartDate { get; set; }

        [Display(Name = "Ngày nghỉ việc")]
        public DateTime? EndDate { get; set; }

        [Display(Name = "Trình độ")]
        public string? Degree { get; set; }
    }
}
