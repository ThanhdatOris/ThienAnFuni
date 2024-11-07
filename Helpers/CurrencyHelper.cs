namespace ThienAnFuni.Helpers
{
    public class CurrencyHelper
    {
        public static string FormatCurrencyVNĐ(double number)
        {
            number = number >= 0 ? number : 0;
            return string.Format("{0:N0}đ", number);
        }
    }
}
