namespace ProductSale.Lib.App.Services
{
    public interface IMailService
    {
        Task<bool> SendEmailAsync(string to, string subject, string body);
    }
}
