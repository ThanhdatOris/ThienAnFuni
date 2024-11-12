namespace ThienAnFuni.Helpers
{
    public static class ConstHelper
    {
        public enum OrderStatus
        {
            Reject = -1,
            Pending = 0,
            Success = 1
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
