using System.Text.RegularExpressions;

namespace ProductSale.Lib.App.Extensions
{
    public static class EmailValidator
    {
        public static bool ValidEmail(this string emailId)
        {
            try
            {
                string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
                return Regex.IsMatch(emailId, pattern, RegexOptions.IgnoreCase);
            }
            catch
            {
                return false;
            }
        }
    }
}
