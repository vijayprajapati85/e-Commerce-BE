using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using ProductSale.Lib.App.Models;

namespace ProductSale.Lib.App.Services
{
    public class MailService : IMailService
    {
        private readonly MailSettings _settings;
        public MailService(IOptions<MailSettings> settings) => _settings = settings.Value;

        public async Task<bool> SendEmailAsync(string to, string subject, string body)
        {
            try
            {
                var email = new MimeMessage();
                email.From.Add(new MailboxAddress(_settings.DisplayName, _settings.Mail));
                email.To.Add(MailboxAddress.Parse(to));
                email.Subject = subject;
                email.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = body };

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
            catch(Exception)
            {

                return false;
            }
        }
    }
}
