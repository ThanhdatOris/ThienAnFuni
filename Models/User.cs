using System.ComponentModel.DataAnnotations;

namespace ThienAnFuni.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên không được để trống")]
        [StringLength(100, ErrorMessage = "Tên không thể dài hơn 100 kí tự")]
        public string? FullName { get; set; }

        public string Username { get; set; }


        [Required(ErrorMessage = "Số điện thoại không được để trống")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        public string? PhoneNumber { get; set; }

        public string? Address { get; set; }

        public string? Gender { get; set; }

        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }

        [Required(ErrorMessage = "Password bắt buộc nhập")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public bool IsActive { get; set; }
        public void ChangePassword() { }
        public void ForgotPassword() { }
        public void UpdateProfile() { }
    }
}
