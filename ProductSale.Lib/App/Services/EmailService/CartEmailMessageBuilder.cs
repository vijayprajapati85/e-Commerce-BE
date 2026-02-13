using ProductSale.Lib.App.Constants;
using ProductSale.Lib.App.Models.Cart;
using ProductSale.Lib.App.Models.Email;
using ProductSale.Lib.Infra.Repo;
using System.Globalization;

namespace ProductSale.Lib.App.Services.EmailService
{
    public class CartEmailMessageBuilder : IEmailMessageBuilder
    {
        private readonly IEmailConfiguration _configuration;
        private readonly ICartInfoRepository _cartrepository;
        public CartEmailMessageBuilder(IEmailConfiguration configuration, ICartInfoRepository cartrepository)
        {
            _configuration = configuration;
            _cartrepository = cartrepository;
        }

        public async Task<IEnumerable<EmailMessage>> BuildAsync(EmailCommand command)
        {
            ICollection<EmailAddress> reciepients = new HashSet<EmailAddress>();

            reciepients?.Add(new EmailAddress
            {
                Name = command.EmailData.GetValueOrDefault("RecipientName") ?? string.Empty,
                Address = command.EmailData.GetValueOrDefault("RecipientEmail") ?? string.Empty
            });

            List<Order> orders = new List<Order>();
            long.TryParse(command.EmailData.GetValueOrDefault("UserId"), out long userId);

            if (userId == 0)
            {
                throw new InvalidOperationException($"Order is not found for the user {userId}");
            }

            orders = await _cartrepository.GetOrderByUserIdAsync(userId) ?? new List<Order>();
            var email = BuildEmail(command, RecipientType.UserEmail, reciepients ?? [], orders);
            var list = new List<EmailMessage>();
            if (email != null)
            {
                list.Add(email);
            }

            return list;
        }

        private EmailMessage? BuildEmail(EmailCommand command, string templateName, ICollection<EmailAddress> recipients, List<Order> orders)
        {
            var template = _configuration.Template(command, templateName);
            var subjectFormat = template.Subject;
            var subject = string.Format(
                provider: CultureInfo.InvariantCulture,
                format: subjectFormat,
                command.EmailData.GetValueOrDefault("RecipientName")
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
                    ["RecipientName"] = recipients.FirstOrDefault()?.Name ?? string.Empty,
                    ["Order"] = orders ?? new List<Order>()
                },
            };

            return email;
        }
    }
}
