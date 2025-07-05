using System.Text.Json.Serialization;

namespace ThienAnFuni.Models.Momo
{
    public class MomoPaymentResponse
    {
        [JsonPropertyName("partnerCode")]
        public string PartnerCode { get; set; } = string.Empty;

        [JsonPropertyName("requestId")]
        public string RequestId { get; set; } = string.Empty;

        [JsonPropertyName("orderId")]
        public string OrderId { get; set; } = string.Empty;

        [JsonPropertyName("amount")]
        public long Amount { get; set; }

        [JsonPropertyName("responseTime")]
        public long ResponseTime { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; } = string.Empty;

        [JsonPropertyName("resultCode")]
        public int ResultCode { get; set; }

        [JsonPropertyName("payUrl")]
        public string PayUrl { get; set; } = string.Empty;

        [JsonPropertyName("deeplink")]
        public string Deeplink { get; set; } = string.Empty;

        [JsonPropertyName("qrCodeUrl")]
        public string QrCodeUrl { get; set; } = string.Empty;

        [JsonPropertyName("deeplinkMiniApp")]
        public string DeeplinkMiniApp { get; set; } = string.Empty;

        // Thêm các trường khác nếu cần dựa trên tài liệu của Momo
    }
}
