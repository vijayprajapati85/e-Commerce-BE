using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using ProductSale.Lib.App.Models;
using ProductSale.Lib.Domain;
using SqlKata.Compilers;
using SqlKata.Execution;
namespace ProductSale.Lib.Infra.Repo
{
    public class SubCategoryRepository : ISubCategoryRepository
    {
        private const string TableName = "SubCategory";

        public QueryFactory queryFactory { get; }
        public SubCategoryRepository(IConfiguration configuration)
        {

            queryFactory = new QueryFactory(
                connection: new SqlConnection(configuration["EcomProduct"]),
                compiler: new SqlServerCompiler()
                );

            //var conn = new MySqlConnection(configuration["EcomProduct"]);
            //queryFactory = new QueryFactory(conn, new MySqlCompiler());
        }

        public async Task<int> UpsertSubCateogryAsync(SubCategory subCategory)
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
        public async Task<SubCategory> GetByIdAsync(long id)
        {

            var result = await queryFactory.Query(TableName)
                 .Where("IsActive", true)
                .Where("Id", id)
                .GetAsync<SubCategory>();

            return result.FirstOrDefault() ?? new SubCategory();
        }
        public async Task<List<SubCategory>> GetAllSubCategoryAsync()
        {

            var results = await queryFactory.Query(TableName)
                .Where("IsActive", true)
                .GetAsync<SubCategory>();

            return results.ToList();
        }
        public async Task<List<SubCategory>> GetByCatIdAsync(long id)
        {

            var result = await queryFactory.Query(TableName)
                 .Where("IsActive", true)
                .Where("CatId", id)
                .GetAsync<SubCategory>();

            return result.ToList();
        }
        public async Task<int> DeleteSubCategoryAsync(DeleteRequest deleteRequest)
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
    }
}
