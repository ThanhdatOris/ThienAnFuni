using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

public static class StringExtensions
{
    public static string ToSlug(this string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return string.Empty;

        // Chuyển chữ thường
        input = input.ToLowerInvariant();

        // Loại bỏ dấu tiếng Việt
        input = input.RemoveVietnameseDiacritics();

        // Loại bỏ ký tự không hợp lệ
        input = Regex.Replace(input, @"[^a-z0-9\s-]", "");

        // Thay khoảng trắng thành dấu gạch ngang
        input = Regex.Replace(input, @"\s+", "-").Trim();

        return input;
    }

    private static string RemoveVietnameseDiacritics(this string text)
    {
        string[] vietnameseChars = new string[] {
            "aàảãáạăằẳẵắặâầẩẫấậ", "dđ", "eèẻẽéẹêềểễếệ", "iìỉĩíị",
            "oòỏõóọôồổỗốộơờởỡớợ", "uùủũúụưừửữứự", "yỳỷỹýỵ"
        };

        string[] replaceChars = new string[] {
            "a", "d", "e", "i", "o", "u", "y"
        };

        for (int i = 0; i < vietnameseChars.Length; i++)
        {
            foreach (var c in vietnameseChars[i].Substring(1))
            {
                text = text.Replace(c, vietnameseChars[i][0]);
            }
        }

        return text;
    }
}
