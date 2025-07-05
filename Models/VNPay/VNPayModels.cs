namespace ThienAnFuni.Models.VNPay
{
    public class VNPayPaymentRequest
    {
        public string TmnCode { get; set; } = string.Empty;
        public long Amount { get; set; }
        public string Command { get; set; } = string.Empty;
        public string CreateDate { get; set; } = string.Empty;
        public string CurrCode { get; set; } = string.Empty;
        public string IpAddr { get; set; } = string.Empty;
        public string Locale { get; set; } = string.Empty;
        public string OrderInfo { get; set; } = string.Empty;
        public string OrderType { get; set; } = string.Empty;
        public string ReturnUrl { get; set; } = string.Empty;
        public string TxnRef { get; set; } = string.Empty;
        public string Version { get; set; } = string.Empty;
        public string SecureHash { get; set; } = string.Empty;
    }

    public class VNPayPaymentResponse
    {
        public string TmnCode { get; set; } = string.Empty;
        public string Amount { get; set; } = string.Empty;
        public string BankCode { get; set; } = string.Empty;
        public string BankTranNo { get; set; } = string.Empty;
        public string CardType { get; set; } = string.Empty;
        public string OrderInfo { get; set; } = string.Empty;
        public string PayDate { get; set; } = string.Empty;
        public string ResponseCode { get; set; } = string.Empty;
        public string TmnCode2 { get; set; } = string.Empty;
        public string TransactionNo { get; set; } = string.Empty;
        public string TransactionStatus { get; set; } = string.Empty;
        public string TxnRef { get; set; } = string.Empty;
        public string SecureHash { get; set; } = string.Empty;
    }
}
