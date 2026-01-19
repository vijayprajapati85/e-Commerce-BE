using DependencyRegister;
using ProductSale.Lib.App.Constants;
using ProductSale.Lib.App.Services;
using ProductSale.Lib.App.Services.EmailService;

namespace ProductSale.ServiceRegisters
{
    public class ServicesRegister : IDependencyRegister
    {
        public void Register(IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<IEmailConfiguration, EmailConfiguration>();
            services.AddTransient<IEmailMessageBuilderFactory, EmailMessageBuilderFactory>();
            services.AddKeyedTransient<IEmailMessageBuilder, SignUpEmailMessageBuilder>(RecipientType.SignUp);
            services.AddKeyedTransient<IEmailMessageBuilder, ResetEmailMessageBuilder>(RecipientType.ForgotPassword);
            services.AddTransient<ITokenService, TokenService>();
            services.AddTransient<IMailService, MailService>();
            services.AddTransient<IProductService, ProductService>();
            services.AddTransient<ICategoryService, CategoryService>();
            services.AddTransient<ISubCategoryService, SubCategoryService>();
            services.AddTransient<IUserInfoService, UserInfoService>();
        }
    }
}
