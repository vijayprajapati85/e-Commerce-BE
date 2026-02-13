using Microsoft.Extensions.Logging;
using ProductSale.Lib.App.Constants;
using ProductSale.Lib.App.Models.Email;
using ProductSale.Lib.Infra.Repo;

namespace ProductSale.Lib.App.Services
{
    public class JobSchedulerService : IJobSchedulerService
    {
        private readonly ICartInfoRepository _cartinforepository;
        private readonly IUserInfoRepository _userrepository;
        private readonly IMailService _emailService;
        private readonly ILogger<JobSchedulerService> _logger;
        public JobSchedulerService(ICartInfoRepository cartinforepository, IUserInfoRepository userrepository, IMailService emailService, ILogger<JobSchedulerService> logger)
        {
            _cartinforepository = cartinforepository;
            _userrepository = userrepository;
            _emailService = emailService;
            _logger = logger;
        }

        public void CartReminderJob()
        {
            _logger.LogInformation("Inside CartReminderJob ===");
            try
            {
                var result = _cartinforepository.GetUserIdPendingCart().Result;
                if (result != null && result.Count > 0)
                {
                    _logger.LogInformation("CartReminderJob: Found {Count} pending carts", result.Count);

                    _logger.LogInformation("CartReminderJob: Sending reminders to users with IDs: {UserIds}", string.Join(", ", result));

                    foreach (var userId in result)
                    {
                        _logger.LogInformation("CartReminderJob: Sending reminder to user with ID: {UserId}", userId);
                        var user = _userrepository.GetUserById(userId).Result;

                        EmailCommand emailCommand = new EmailCommand
                        {
                            EmailType = RecipientType.Cart,
                            EmailData = new Dictionary<string, string>
                            {
                                { "RecipientName", user.FullName },
                                { "RecipientEmail", user.EmailId },
                                { "UserId", userId.ToString() },
                            }
                        };

                        var isEmailSend = _emailService.SendEmailAsync(emailCommand).Result;
                    }

                }
                else
                {
                    _logger.LogInformation("CartReminderJob: No pending carts found");
                }

            }
            catch (Exception ex)
            {
                _logger.LogError("Error in CartReminderJob: {Message}", ex.Message);
            }
        }

        public void NewProductReminderJob()
        {
            _logger.LogInformation("Inside NewProductReminderJob ===");
            try
            {
                EmailCommand emailCommand = new EmailCommand
                {
                    EmailType = RecipientType.NewProducts
                };

                var isEmailSend = _emailService.SendEmailAsync(emailCommand).Result;

            }
            catch (Exception ex)
            {
                _logger.LogError("Error in CartReminderJob: {Message}", ex.Message);
            }
        }
    }
}
