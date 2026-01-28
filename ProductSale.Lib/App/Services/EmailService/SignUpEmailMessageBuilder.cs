using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ProductSale.Lib.App.Constants;
using ProductSale.Lib.App.Models.Email;

namespace ProductSale.Lib.App.Services.EmailService
{
    public class SignUpEmailMessageBuilder : IEmailMessageBuilder
    {
        private readonly IEmailConfiguration _configuration;
        public SignUpEmailMessageBuilder(IEmailConfiguration configuration)
        {
            _configuration = configuration;
        }

        public Task<IEnumerable<EmailMessage>> BuildAsync(EmailCommand command)
        {
            ICollection<EmailAddress> reciepients = new HashSet<EmailAddress>();

            reciepients?.Add(new EmailAddress
            {
                Name = command.EmailData.GetValueOrDefault("RecipientName") ?? string.Empty,
                Address = command.EmailData.GetValueOrDefault("RecipientEmail") ?? string.Empty
            });

            var email = BuildEmail(command, RecipientType.UserEmail, reciepients ?? []);
            var list = new List<EmailMessage>();
            if (email != null)
            {
                list.Add(email);
            }
            return Task.FromResult<IEnumerable<EmailMessage>>(list);
        }

        private EmailMessage? BuildEmail(EmailCommand command, string templateName, ICollection<EmailAddress> recipients)
        {
            var template = _configuration.Template(command, templateName);
            var subjectFormat = template.Subject;

            if (string.IsNullOrWhiteSpace(subjectFormat))
            {
                return null;
            }

            EmailMessage email = new()
            {
                Recipients = recipients,
                Subject = subjectFormat,
                TemplateFile = template.TemplateFile,
                Data =
                {
                    ["RecipientName"] = recipients.FirstOrDefault()?.Name ?? string.Empty,
                    ["Password"] = command.EmailData.GetValueOrDefault("Password") ?? string.Empty,
                },
            };

            return email;
        }
    }
}
