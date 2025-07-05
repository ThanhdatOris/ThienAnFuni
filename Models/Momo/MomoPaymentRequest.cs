using System.Text.Json.Serialization;

namespace ThienAnFuni.Models.Momo
{
    public class MomoPaymentRequest
    {
        [JsonPropertyName("partnerCode")]
        public string PartnerCode { get; set; } = string.Empty;

        [JsonPropertyName("partnerName")]
        public string PartnerName { get; set; } = "Test"; // Có thể cấu hình hoặc để mặc định

        [JsonPropertyName("storeId")]
        public string StoreId { get; set; } = "MomoTestStore"; // Có thể cấu hình hoặc để mặc định

        [JsonPropertyName("requestId")]
        public string RequestId { get; set; } = string.Empty;

        [JsonPropertyName("amount")]
        public long Amount { get; set; } // Momo thường yêu cầu amount là kiểu số nguyên

        [JsonPropertyName("orderId")]
        public string OrderId { get; set; } = string.Empty;

        [JsonPropertyName("orderInfo")]
        public string OrderInfo { get; set; } = string.Empty;

        [JsonPropertyName("redirectUrl")]
        public string RedirectUrl { get; set; } = string.Empty;

        [JsonPropertyName("ipnUrl")]
        public string IpnUrl { get; set; } = string.Empty;

        [JsonPropertyName("lang")]
        public string Lang { get; set; } = "vi";

        [JsonPropertyName("extraData")]
        public string ExtraData { get; set; } = string.Empty; // Base64 encoded string if not empty

        [JsonPropertyName("requestType")]
        public string RequestType { get; set; } = "captureWallet"; // Hoặc "payWithATM" tùy theo loại thanh toán

        [JsonPropertyName("accessKey")]
        public string AccessKey { get; set; } = string.Empty;

        [JsonPropertyName("signature")]
        public string Signature { get; set; } = string.Empty;
    }
}
