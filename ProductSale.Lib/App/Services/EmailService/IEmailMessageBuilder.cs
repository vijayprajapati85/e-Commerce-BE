using ProductSale.Lib.App.Models.Email;

namespace ProductSale.Lib.App.Services.EmailService
{
    public interface IEmailMessageBuilder
    {
        Task<IEnumerable<EmailMessage>> BuildAsync(EmailCommand command);
    }
}
