using ProductSale.Lib.App.Constants;
using ProductSale.Lib.App.Models;
using ProductSale.Lib.App.Models.Cart;
using ProductSale.Lib.App.Models.Email;
using ProductSale.Lib.Infra.Repo;
using System.Globalization;

namespace ProductSale.Lib.App.Services.EmailService
{
    public class NewProductEmailMessageBuilder : IEmailMessageBuilder
    {
        private readonly IEmailConfiguration _configuration;
        private readonly IProductRepository _productrepository;
        private readonly IUserInfoRepository _userInfoRepository;
        public NewProductEmailMessageBuilder(IEmailConfiguration configuration, IProductRepository productrepository, IUserInfoRepository userInfoRepository)
        {
            _configuration = configuration;
            _productrepository = productrepository;
            _userInfoRepository = userInfoRepository;
        }

        public async Task<IEnumerable<EmailMessage>> BuildAsync(EmailCommand command)
        {
            ICollection<EmailAddress> reciepients = new HashSet<EmailAddress>();

            var recipientEmail = await _userInfoRepository.GetAllUsers();

            if(recipientEmail == null || !recipientEmail.Any()) 
            {
                throw new InvalidOperationException("No users found to send the email."); 
            }

            foreach (var user in recipientEmail) 
            {
                reciepients.Add(new EmailAddress {
                    Name = user.FullName,
                    Address = user.EmailId
                }); 
            }

            List<ProductInfoDto> products = new List<ProductInfoDto>();
            products = await _productrepository.GetNewProduct() ?? new List<ProductInfoDto>();
            if (products == null || !products.Any())
            {
                throw new InvalidOperationException("No new product found to send the email.");
            }

            var email = BuildEmail(command, RecipientType.UserEmail, reciepients ?? [], products);
            var list = new List<EmailMessage>();
            if (email != null)
            {
                list.Add(email);
            }

            return list;
        }

        private EmailMessage? BuildEmail(EmailCommand command, string templateName, ICollection<EmailAddress> recipients, List<ProductInfoDto> products)
        {
            var template = _configuration.Template(command, templateName);
            var subjectFormat = template.Subject;
            var subject = string.Format(
                provider: CultureInfo.InvariantCulture,
                format: subjectFormat
                );

            if (string.IsNullOrWhiteSpace(subjectFormat))
            {
                return null;
            }

            EmailMessage email = new()
            {
                Recipients = recipients,
                Subject = subject,
                TemplateFile = template.TemplateFile,
                Data =
                {
                    ["Products"] = products ?? new List<ProductInfoDto>()
                },
            };

            return email;
        }
    }
}
