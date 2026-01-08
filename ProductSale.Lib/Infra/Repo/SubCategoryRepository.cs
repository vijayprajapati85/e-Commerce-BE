using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ProductSale.Lib.App.Models;
using ProductSale.Lib.Domain;
using SqlKata.Compilers;
using SqlKata.Execution;
namespace ProductSale.Lib.Infra.Repo
{
    public class SubCategoryRepository : ISubCategoryRepository
    {
        private const string TableName = "SubCategory";
        private readonly ILogger<SubCategoryRepository> _logger;
        public QueryFactory queryFactory { get; }
        public SubCategoryRepository(IConfiguration configuration, ILogger<SubCategoryRepository> logger)
        {

            queryFactory = new QueryFactory(
                connection: new SqlConnection(configuration["EcomProduct"]),
                compiler: new SqlServerCompiler()
                );

            _logger = logger;
        }

        public async Task<int> UpsertSubCateogryAsync(SubCategory subCategory)
        {
            _logger.LogInformation("Inside UpsertSubCateogryAsync ===");
            try
            {
                if (subCategory.Id == 0)
                {
                    return await queryFactory.StatementAsync("exec InsertSubCategories @p_Name,@p_CatId,@p_UserId", new { p_Name = subCategory.Name, p_CatId = subCategory.CatId, p_UserId = subCategory.CreatedBy });
                }

                return await queryFactory.Query(TableName)
                    .Where("Id", subCategory.Id)
                    .UpdateAsync(new
                    {
                        Name = subCategory.Name,
                        CatId = subCategory.CatId,
                        IsActive = subCategory.IsActive,
                        UpdatedBy = subCategory.UpdatedBy,
                        UpdatedDateTime = subCategory.UpdatedDateTime,
                    });
            }
            catch (Exception ex)
            {
                _logger.LogError("Inside UpsertSubCateogryAsync === {error}", ex);
                return 0;
            }
        }
        public async Task<SubCategory> GetByIdAsync(long id)
        {
            _logger.LogInformation("Inside GetByIdAsync ===");
            try
            {
                var result = await queryFactory.Query(TableName)
                     .Where("IsActive", true)
                    .Where("Id", id)
                    .GetAsync<SubCategory>();

                return result.FirstOrDefault() ?? new SubCategory();
            }
            catch(Exception ex)
            {
                _logger.LogError("Inside GetByIdAsync === {error}", ex);
                return new SubCategory();
            }
        }
        public async Task<List<SubCategory>> GetAllSubCategoryAsync()
        {
            _logger.LogInformation("Inside GetAllSubCategoryAsync ===");
            try
            {
                var results = await queryFactory.Query(TableName)
                    .Where("IsActive", true)
                    .GetAsync<SubCategory>();

                return results.ToList();
            }
            catch(Exception ex)
            {
                _logger.LogError("Inside GetAllSubCategoryAsync === {error}", ex);
                return new List<SubCategory>();
            }
        }
        public async Task<List<SubCategory>> GetByCatIdAsync(long id)
        {
            _logger.LogInformation("Inside GetByCatIdAsync ===");
            try
            {
                var result = await queryFactory.Query(TableName)
                     .Where("IsActive", true)
                    .Where("CatId", id)
                    .GetAsync<SubCategory>();

                return result.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError("Inside GetByCatIdAsync === {error}", ex);
                return new List<SubCategory>();
            }
        }
        public async Task<int> DeleteSubCategoryAsync(DeleteRequest deleteRequest)
        {
            _logger.LogInformation("Inside DeleteSubCategoryAsync ===");
            try
            {
                return await queryFactory.Query(TableName)
                    .Where("Id", deleteRequest.Id)
                    .UpdateAsync(new
                    {
                        IsActive = false,
                        UpdatedBy = deleteRequest.UpdatedBy,
                        UpdatedDateTime = DateTime.Now,
                    });
            }
            catch (Exception ex)
            {
                _logger.LogError("Inside DeleteSubCategoryAsync === {error}", ex);
                return 0;
            }
        }
    }
}
