using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ProductSale.Lib.App.Models;
using ProductSale.Lib.Domain;
using SqlKata;
using SqlKata.Compilers;
using SqlKata.Execution;
namespace ProductSale.Lib.Infra.Repo
{
    public class CategoryRepository : ICategoryRepository
    {
        private const string TableName = "Category";
        private const string SubCategory = "SubCategory";
        private readonly ILogger<CategoryRepository> _logger;

        public QueryFactory queryFactory { get; }
        public CategoryRepository(IConfiguration configuration, ILogger<CategoryRepository> logger)
        {

            queryFactory = new QueryFactory(
                connection: new SqlConnection(configuration["EcomProduct"]),
                compiler: new SqlServerCompiler()
                );

            _logger = logger;
        }

        public async Task<int> UpsertCateogryAsync(Category category)
        {
            _logger.LogInformation("Inside UpsertCateogryAsync ===");
            try
            {
                if (category.Id == 0)
                {
                    return await queryFactory.StatementAsync("exec InsertCategories @p_Name,@p_UserId", new { p_Name = category.Name, p_UserId = category.CreatedBy });
                }

                return await queryFactory.Query(TableName)
                    .Where("Id", category.Id)
                    .UpdateAsync(new
                    {
                        Name = category.Name,
                        IsActive = category.IsActive,
                        UpdatedBy = category.UpdatedBy,
                        UpdatedDateTime = category.UpdatedDateTime,
                    });
            }
            catch (Exception ex)
            {
                _logger.LogError("Inside UpsertCateogryAsync === {error}", ex);
                return 0;
            }
        }
        public async Task<Category> GetByIdAsync(long id)
        {
            _logger.LogInformation("Inside GetByIdAsync ===");
            try
            {
                var result = await queryFactory.Query(TableName)
                     .Where("IsActive", true)
                    .Where("Id", id)
                    .GetAsync<Category>();

                return result.FirstOrDefault() ?? new Category();
            }
            catch(Exception ex)
            {
                _logger.LogError("Inside GetByIdAsync === {error}", ex);
                return new Category();
            }
        }
        public async Task<List<Category>> GetAllCategoryAsync()
        {
            _logger.LogInformation("Inside GetAllCategoryAsync ===");

            try
            {
                   var results = queryFactory.Query(TableName)
                    .LeftJoin(SubCategory, join => join.On($"{SubCategory}.CatId", $"{TableName}.Id").Where($"{SubCategory}.IsActive", true))
                    .Where($"{TableName}.IsActive", true)
                    .Select($"{TableName}.*", $"{SubCategory}.*");

                var compiler = new SqlServerCompiler();
                SqlResult result = compiler.Compile(results);


                string sql = result.Sql;
                object parameters = new DynamicParameters(result.Bindings);
                var postDictionary = new Dictionary<int, Category>();
                using (var connection = queryFactory.Connection)
                {
                    var data = await connection.QueryAsync<Category, SubCategory, Category>(
                        sql,
                        (category, subCategory) =>
                        {
                            if (!postDictionary.TryGetValue((int)category.Id, out var currentCategory))
                            {
                                currentCategory = category;
                                currentCategory.SubCategories = new List<SubCategory>();
                                postDictionary.Add((int)currentCategory.Id, currentCategory);
                            }
                            if (subCategory != null)
                            {
                                if (subCategory.Id != 0)
                                {
                                    currentCategory.SubCategories!.Add(subCategory);
                                }
                            }

                            return currentCategory;
                        },
                        parameters,
                        splitOn: "Id"
                    );

                    return postDictionary.Values.ToList();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Inside GetAllCategoryAsync === {error}", ex);
                return new List<Category>();
            }
        }
        public async Task<int> DeleteCategoryAsync(DeleteRequest deleteRequest)
        {
            _logger.LogInformation("Inside DeleteCategoryAsync ===");
            try
            {
                return await queryFactory.Query(TableName)
                    .Where("Id", deleteRequest.Id)
                    .UpdateAsync(new
                    {
                        IsActive = false,
                        Updatedby = deleteRequest.UpdatedBy,
                        UpdatedDateTime = DateTime.Now,
                    });
            }
            catch(Exception ex)
            {
                _logger.LogError("Inside DeleteCategoryAsync === {error}", ex);
                return 0;
            }
        }
    }
}
