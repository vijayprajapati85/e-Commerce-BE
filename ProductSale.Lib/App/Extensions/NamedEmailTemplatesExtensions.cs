using ProductSale.Lib.App.Models.Email;
using ProductSale.Lib.App.Services.EmailService;

namespace ProductSale.Lib.App.Extensions
{
    public static class NamedEmailTemplatesExtensions
    {
        public static EmailTemplate Get(
            this NamedEmailTemplates templates,
            string name)
        {
            return templates.TryGetValue(name, out var value) ? value : new EmailTemplate();
        }
    }
}
