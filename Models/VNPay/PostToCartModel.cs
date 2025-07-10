namespace ThienAnFuni.Models.VNPay
{
    public class PostToCartModel
    {
        public string TxnRef { get; set; } = string.Empty;
        public string Amount { get; set; } = string.Empty;
        public string OrderInfo { get; set; } = string.Empty;
        public string TransactionNo { get; set; } = string.Empty;
        public string BankCode { get; set; } = string.Empty;
        public string PayDate { get; set; } = string.Empty;
    }
}
