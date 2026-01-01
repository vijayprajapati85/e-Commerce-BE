using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
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

        public QueryFactory queryFactory { get; }
        public CategoryRepository(IConfiguration configuration)
        {

            queryFactory = new QueryFactory(
                connection: new SqlConnection(configuration["EcomProduct"]),
                compiler: new SqlServerCompiler()
                );

            //var conn = new MySqlConnection(configuration["EcomProduct"]);
            //queryFactory = new QueryFactory(conn, new MySqlCompiler());
        }

        public async Task<int> UpsertCateogryAsync(Category category)
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
        public async Task<Category> GetByIdAsync(long id)
        {

            var result = await queryFactory.Query(TableName)
                 .Where("IsActive", true)
                .Where("Id", id)
                .GetAsync<Category>();

            return result.FirstOrDefault() ?? new Category();
        }
        public async Task<List<Category>> GetAllCategoryAsync()
        {

            try
            {

                // $"{SubCategory}.CatId", $"{TableName}.Id"
                var results = queryFactory.Query(TableName)
                    .LeftJoin(SubCategory, join => join.On($"{SubCategory}.CatId", $"{TableName}.Id").Where($"{SubCategory}.IsActive", true))
                    .Where($"{TableName}.IsActive", true)
                    .Select($"{TableName}.*", $"{SubCategory}.*");

                //var results = new queryFactory.Query($"{TableName} as c")
                //   .LeftJoin($"{SubCategory} as s", join => join
                //       .On("s.CatId", "c.Id")
                //       .Where("s.IsActive", 1)
                //   )
                //   .Where("c.IsActive", 1)
                //   .Select($"{TableName}.*", $"{SubCategory}.*");

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
                            // Guard against null or empty mapped SubCategory (left join with no match)
                            if (subCategory != null)
                            {
                                // If Dapper created a default SubCategory with Id == 0 for no-match rows,
                                // skip adding that empty object. Adjust property check if your Id type differs.
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
                Console.WriteLine(ex.Message);
            }

            return null;
        }
        public async Task<int> DeleteCategoryAsync(DeleteRequest deleteRequest)
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
    }
}
