using System.Security.Cryptography;

namespace ProductSale.Lib.App.Extensions
{
    public static class PasswordGeneratHelper
    {
        public static string GeneratePassword()
        {
            string allowedChars = "ABCDEFGHJKLMNOPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz0123456789!@$?_-";
            int passwordLength = 7;
            string password = RandomNumberGenerator.GetString(allowedChars, passwordLength);
            return password;
        }
    }
}
