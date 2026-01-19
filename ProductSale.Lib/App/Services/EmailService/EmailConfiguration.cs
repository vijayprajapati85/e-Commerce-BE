using Microsoft.Extensions.Configuration;
using ProductSale.Lib.App.Extensions;
using ProductSale.Lib.App.Models.Email;

namespace ProductSale.Lib.App.Services.EmailService
{
    public class EmailConfiguration(IConfiguration configuration) : IEmailConfiguration
    {
        public EmailTemplate Template(EmailCommand command, string name)
        {
            return NamedTemplates(name).Get(command.EmailType);
        }
        private NamedEmailTemplates NamedTemplates(string name)
        {
            NamedEmailTemplates templates = [];
            configuration.GetSection($"Template:{name}")
                .Bind(templates);

            return templates;
        }
    }
}
