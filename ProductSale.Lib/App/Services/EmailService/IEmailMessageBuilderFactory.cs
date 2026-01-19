namespace ProductSale.Lib.App.Services.EmailService
{
    public interface IEmailMessageBuilderFactory
    {
        IEmailMessageBuilder? Create(string emailType);
    }
}
