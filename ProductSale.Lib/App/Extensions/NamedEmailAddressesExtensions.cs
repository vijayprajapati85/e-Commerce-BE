using ProductSale.Lib.App.Models.Email;
using ProductSale.Lib.App.Services.EmailService;
using System.Dynamic;

namespace ProductSale.Lib.App.Extensions
{
    public static class NamedEmailAddressesExtensions
    {
        public static List<EmailAddress> Get(
            this NamedEmailAddresses addresses,
            string name)
        {
            return addresses.TryGetValue(name, out var value) ? value : [];
        }
    }
}
