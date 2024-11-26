using System.Security.Policy;

namespace ThienAnFuni.Helpers
{
    public static class ConstHelper
    {
        // OrderStatus
        public enum OrderStatus
        {
            Reject = -1,
            Pending = 0,
            Success = 1
        }
        public static string GetOrderStatus(OrderStatus status)
        {
            return status switch
            {
                OrderStatus.Reject => "Đã hủy",
                OrderStatus.Pending => "Chờ duyệt",
                OrderStatus.Success => "Giao thành công",
                _ => "Trạng thái không xác định"
            };
        }

        // PaymentStatus
        public enum PaymentStatus
        {
            Unpaid = 0,    // Chưa thanh toán
            Paid = 1       // Đã thanh toán
        }

        public static string GetPaymentStatus(PaymentStatus status)
        {
            return status switch
            {
                PaymentStatus.Unpaid => "Chưa thanh toán",
                PaymentStatus.Paid => "Đã thanh toán",
                _ => "Chưa xác định"
            };
        }

        // PaymentMethod
        public enum PaymentMethod
        {
            Cash = 0,   
            Bank_transfer = 1 
        }

        public static string GetPaymentMethod(PaymentMethod status)
        {
            return status switch
            {
                PaymentMethod.Cash => "Tiền mặt",
                PaymentMethod.Bank_transfer => "Chuyển khoản",
                _ => "Phương thức thanh toán không xác định"
            };
        }

        public enum Roles
        {
            Admin = 0,
            SaleStaff = 1,
            Customer = 2
        }

        // Định nghĩa hằng số cho các role
        public const string RoleManager = "Manager";
        public const string RoleSaleStaff = "SaleStaff";
        public const string RoleCustomer = "Customer";

        // Mảng chứa tất cả các role hằng số
        public static readonly string[] AllRoles = { RoleManager, RoleSaleStaff, RoleCustomer };
    }
}
