using Microsoft.Extensions.DependencyInjection;
using ProductSale.Lib.App.Services.EmailService;

public class EmailMessageBuilderFactory(IServiceProvider serviceProvider) : IEmailMessageBuilderFactory
{
    public IEmailMessageBuilder? Create(string emailType)
    {
        return serviceProvider.GetKeyedService<IEmailMessageBuilder>(emailType);
    }
}
