using FluentEmail.Core;
using FluentEmail.Core.Models;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using ProductSale.Lib.App.Extensions;
using ProductSale.Lib.App.Models;
using ProductSale.Lib.App.Models.Email;
using ProductSale.Lib.App.Services.EmailService;

namespace ProductSale.Lib.App.Services
{
    public class MailService : IMailService
    {
        private readonly MailSettings _settings;
        private readonly IEmailMessageBuilderFactory _builderFactory;
        private readonly ILogger<MailService> _logger;
        private readonly IFluentEmail _fluentEmail;
        public MailService(IOptions<MailSettings> settings, IEmailMessageBuilderFactory builderFactory, ILogger<MailService> logger, IFluentEmail fluentEmail)
        {
            _settings = settings.Value;
            _builderFactory = builderFactory;
            _logger = logger;
            _fluentEmail = fluentEmail;
        }
        public async Task<bool> SendEmailAsync(EmailCommand emailCommand)
        {
            try
            {
                //Build template
                var emailMessageBuilder = _builderFactory.Create(emailCommand.EmailType);
                if (emailMessageBuilder == null)
                {
                    _logger.LogInformation("SendEmailAsync === emailMessageBuilder is Null");
                    return false;
                }

                var emails = await emailMessageBuilder.BuildAsync(emailCommand);

                var result = false;

                foreach(var email in emails)
                {
                    result = await SendEmailFluent(email);
                }
                return result;
            }
            catch(Exception)
            {
                return false;
            }
        }
        private async Task<bool> SendEmailFluent(EmailMessage message)
        {
            try
            {
                var email = _fluentEmail.Subject(message.Subject);

                email.To(message.Recipients.Select(x => new Address(x.Address, x.Name)));

                if (!string.IsNullOrWhiteSpace(message.TemplateFile))
                {
                    var templatePath = Path.Combine(AppContext.BaseDirectory, "EmailTemplate", message.TemplateFile);

                    _logger.LogInformation("SendEmailFluent === Using template file at {TemplatePath}", templatePath);

                    email.UsingTemplateFromFile(
                        filename: templatePath,
                        model: message.TemplateData());
                }

                var result = await _fluentEmail.SendAsync();

                _logger.LogInformation("SendEmailFluent === Email sent to {Recipients} with subject {Subject}", 
                    string.Join(", ", message.Recipients.Select(r => r.Address)), message.Subject);

                _logger.LogInformation("SendEmailFluent === Result: {Result}", result.Successful ? "Successful" : "Failed");    

                if (result.Successful)
                    return true; 
            }
            catch (Exception ex)
            {
               _logger.LogInformation(ex, "Error in SendEmailFluent");
                return false;
            }

            return false;
        }
        private async Task<bool> SendEmail(EmailMessage message)
        {
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress(_settings.DisplayName, _settings.Mail));
            foreach (var item in message.Recipients)
            {
                email.To.Add(MailboxAddress.Parse(item.Address));
            }

            email.Subject = message.Subject;

            if (!string.IsNullOrWhiteSpace(message.TemplateFile))
            {
                var templatePath = Path.Combine(AppContext.BaseDirectory, "EmailTemplate", message.TemplateFile);
              //  email
            }
            
            email.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = message.Body };

            using (var smtp = new SmtpClient())
            {
                try
                {
                    await smtp.ConnectAsync(_settings.Host, _settings.Port, SecureSocketOptions.StartTls);
                    //await smtp.ConnectAsync(_settings.Host, _settings.Port, SecureSocketOptions.None);
                    // If you don't use credentials, you can skip this
                    if (!string.IsNullOrEmpty(_settings.Password))
                    {
                        await smtp.AuthenticateAsync(_settings.Mail, _settings.Password);
                    }
                    await smtp.SendAsync(email);
                    return true;
                }
                catch (Exception ex)
                {
                    // Log the exception here
                    Console.WriteLine($"Error sending email: {ex.Message}");
                    throw;
                }
                finally
                {
                    await smtp.DisconnectAsync(true);
                }
            }
        }
    }
}
