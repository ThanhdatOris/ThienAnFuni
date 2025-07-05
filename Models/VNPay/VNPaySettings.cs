namespace ThienAnFuni.Models.VNPay
{
    public class VNPaySettings
    {
        public string TmnCode { get; set; } = string.Empty; // Terminal ID
        public string HashSecret { get; set; } = string.Empty; // Secret Key
        public string PaymentUrl { get; set; } = string.Empty; // Payment URL
        public string ReturnUrl { get; set; } = string.Empty; // Return URL
        public string Version { get; set; } = "2.1.0"; // API Version
        public string Command { get; set; } = "pay"; // Command
        public string CurrCode { get; set; } = "VND"; // Currency
        public string Locale { get; set; } = "vn"; // Locale
    }
}
