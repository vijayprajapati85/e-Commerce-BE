using ProductSale.Lib.App.Models.Email;

namespace ProductSale.Lib.App.Services.EmailService
{
    public class NamedEmailTemplates() : Dictionary<string, EmailTemplate>(StringComparer.OrdinalIgnoreCase);
}
