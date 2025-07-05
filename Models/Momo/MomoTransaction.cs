using System.ComponentModel.DataAnnotations;

namespace ThienAnFuni.Models.Momo
{
    public class MomoTransaction
    {
        [Key]
        public int Id { get; set; }

        public string PartnerCode { get; set; }
        public string OrderId { get; set; } // Mã đơn hàng của bạn
        public string MomoRequestId { get; set; } // RequestId gửi cho Momo
        public long Amount { get; set; }
        public string OrderInfo { get; set; }
        public int ResultCode { get; set; } // Mã kết quả từ Momo
        public string Message { get; set; } // Thông báo từ Momo
        public string PayUrl { get; set; } // Link thanh toán Momo trả về
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastUpdatedAt { get; set; } // Thời gian cập nhật cuối từ IPN

        // public string UserId { get; set; } // Nếu liên kết với User của bạn
        // [ForeignKey("UserId")]
        // public virtual ApplicationUser User { get; set; } // Giả sử bạn có ApplicationUser
    }
}
