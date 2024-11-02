using Microsoft.AspNetCore.Identity;

namespace ThienAnFuni.Helpers
{
    public class PasswordHelper
    {
        public static string HashPassword(string password)
        {
            var passwordHasher = new PasswordHasher<object>();
            return passwordHasher.HashPassword(null, password);
        }
    }
}
