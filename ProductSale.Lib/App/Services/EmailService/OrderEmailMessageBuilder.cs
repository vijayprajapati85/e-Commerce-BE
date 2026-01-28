using ProductSale.Lib.App.Constants;
using ProductSale.Lib.App.Models.Cart;
using ProductSale.Lib.App.Models.Email;
using ProductSale.Lib.Infra.Repo;
using System.Globalization;
using System.Linq;

namespace ProductSale.Lib.App.Services.EmailService
{
    public class OrderEmailMessageBuilder : IEmailMessageBuilder
    {
        private readonly IEmailConfiguration _configuration;
        private readonly ICartInfoRepository _cartrepository;
        public OrderEmailMessageBuilder(IEmailConfiguration configuration, ICartInfoRepository cartrepository)
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
            
            if(userId == 0)
            {
                throw new InvalidOperationException($"Order is not found for the user {userId}");
            }

            string orderId = command.EmailData.GetValueOrDefault("OrderId") ?? string.Empty;
            orders = await _cartrepository.GetOrderByUserIdAsync(userId, orderId) ?? new List<Order>();
            var email = BuildEmail(command, RecipientType.UserEmail, reciepients ?? [], orders);
            var list = new List<EmailMessage>();
            if (email != null)
            {
                list.Add(email);
            }

            var recipient = _configuration.Recipients(RecipientType.OrderTeam);
            var adminEmail = BuildEmail(command, RecipientType.AdminEmail, recipient, orders);
            if (adminEmail != null)
            {
                list.Add(adminEmail);
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
                command.EmailData.GetValueOrDefault("RecipientName"),
                orders?.Select(x=> x.OrderId).FirstOrDefault()
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
