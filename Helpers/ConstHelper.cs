namespace ThienAnFuni.Helpers
{
    public class ConstHelper
    {
        //public static class OrderStatusConstants
        //{
        //    public const int Reject = -1;
        //    public const int Pending = 0;
        //    public const int Success = 1;
        //}
        public enum OrderStatus
        {
            Reject = -1,
            Pending = 0,
            Success = 1
        }

        //public static class RolesConstants 
        //{
        //    public const int Admin = 0;
        //    public const int SaleStaff = 1;
        //    public const int Customer = 2;
        //}
        public enum Roles
        {
            Admin = 0,
            SaleStaff = 1,
            Customer = 2
        }

    }
}
