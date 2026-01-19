using ProductSale.Lib.App.Models.Email;

namespace ProductSale.Lib.App.Services.EmailService
{
    public interface IEmailConfiguration
    {
        public EmailTemplate Template(EmailCommand command, string name);
    }
}
