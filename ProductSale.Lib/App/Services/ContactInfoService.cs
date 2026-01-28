using Microsoft.Extensions.Logging;
using ProductSale.Lib.App.Exceptions;
using ProductSale.Lib.App.Extensions;
using ProductSale.Lib.App.Models;
using ProductSale.Lib.App.Models.Email;
using ProductSale.Lib.Infra.Repo;

namespace ProductSale.Lib.App.Services
{
    public class ContactInfoService : IContactInfoService
    {

        private readonly IContactInfoRepository _repository;
        private readonly ILogger<ContactInfoService> _logger;

        public long UserId { get; set; }
        public ContactInfoService(IContactInfoRepository repository, ILogger<ContactInfoService> logger)
        {
            _repository = repository;
            _logger = logger;
        }
        public async Task<int> Submit(ContactInfo contactInfo)
        {
            _logger.LogInformation("Inside ContactInfo Submit ===");
            try
            {
                bool validEmail = contactInfo.Email.ValidEmail();
                if (!validEmail)
                {
                    throw new BusinessRuleException("Email address is not valid.");
                }

                int result = 0;

                result = await _repository.Submit(contactInfo);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in ContactInfo Submit: {Message}", ex.Message);
                return 0;
            }
        }
    }
}
