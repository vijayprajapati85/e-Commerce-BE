
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ProductSale.Lib.App.Models.Cart;
using ProductSale.Lib.App.Models.Email;
using SqlKata.Compilers;
using SqlKata.Execution;

namespace ProductSale.Lib.Infra.Repo
{
    public class ContactInfoRepository : IContactInfoRepository
    {
        private const string TableName = "ContactInfo";
        private readonly ILogger<ContactInfoRepository> _logger;

        public QueryFactory queryFactory { get; }
        public ContactInfoRepository(IConfiguration configuration, ILogger<ContactInfoRepository> logger)
        {

            queryFactory = new QueryFactory(
                connection: new SqlConnection(configuration["EcomProduct"]),
                compiler: new SqlServerCompiler()
                );

            _logger = logger;
        }
        public async Task<int> Submit(ContactInfo contactInfo)
        {
            _logger.LogInformation("Inside ContactInfo Submit ===");
            try
            {
                return await queryFactory.Query(TableName)
                    .InsertAsync(new
                    {
                        Name = contactInfo.Name,
                        Email = contactInfo.Email,
                        Message = contactInfo.Message,
                        CreatedDateTime = DateTime.Now
                    });
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in ContactInfo Submit: {Message}", ex.Message);
                return 0;
            }
        }
    }
}
