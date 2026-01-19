using ProductSale.Lib.App.Models.Email;

namespace ProductSale.Lib.App.Services
{
    public interface IMailService
    {
        Task<bool> SendEmailAsync(EmailCommand emailCommand);
    }
}
