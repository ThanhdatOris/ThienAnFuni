namespace ThienAnFuni.Helpers
{
    public class OrderStatusHelper
    {
        //public static string ReturnStatusSN(string status)
        //{
        //    return status switch
        //    {
        //        "pending" => "Đang xử lý",
        //        "success" => "Đã giao hàng",
        //        "cancel" => "Đã hủy đơn",
        //        _ => "Không xác định"
        //    };
        //}

        //public static string ReturnCssStatusSN(string status)
        //{
        //    status = status.ToLower();

        //    return status switch
        //    {
        //        "pending" => "text-warning",
        //        "success" => "text-success",
        //        _ => "text-danger"
        //    };
        //}
        public static string ReturnStatus(string status)
        {
            return status switch
            {
                "0" => "Đang xử lý",
                "1" => "Đã giao hàng",
                "-1" => "Đã hủy đơn",
                _ => "Không xác định"
            };
        }

        public static string ReturnCssStatus(string status)
        {
            return status switch
            {
                "0" => "text-warning",
                "1" => "text-success",
                _ => "text-danger"
            };
        }
    }
}
