namespace ThienAnFuni.Helpers
{
    public class OrderStatusHelper
    {
        public static string ReturnStatus(string status)
        {
            return status switch
            {
                "pending" => "Đang xử lý",
                "success" => "Đã giao hàng",
                "cancel" => "Đã hủy đơn",
                _ => "Không xác định"
            };
        }

        public static string ReturnCssStatus(string status)
        {
            status = status.ToLower();

            return status switch
            {
                "pending" => "text-warning",
                "success" => "text-success",
                _ => "text-danger"
            };
        }
    }
}
