using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using ProductSale.Domain;
using ProductSale.Lib.App.Extensions;
using ProductSale.Lib.App.Models;
using SqlKata.Compilers;
using SqlKata.Execution;
namespace ProductSale.Lib.Infra.Repo
{
    public class ProductRepository : IProductRepository
    {
        private const string TableName = "ProductInfo";
        private const string CatTable = "Category";
        private const string SubCatTable = "SubCategory";

        public QueryFactory queryFactory { get; }
        public ProductRepository(IConfiguration configuration)
        {

            queryFactory = new QueryFactory(
                connection: new SqlConnection(configuration["EcomProduct"]),
                compiler: new SqlServerCompiler()
                );
            //var conn = new MySqlConnection(configuration["EcomProduct"]);
            //queryFactory = new QueryFactory(conn, new MySqlCompiler());
        }

        public async Task<int> UpsertProductAsync(ProductInfo product)
        {

            if (product.Id == 0)
            {
                return await queryFactory.Query(TableName).InsertAsync(new
                {
                    Name = product.Name,
                    CatId = product.CatId,
                    SubCatId = product.SubCatId,
                    Description = product.Description,
                    Price = product.Price,
                    ImageName = product.ImageName,
                    IsActive = product.IsActive,
                    CreatedBy = product.CreatedBy,
                    CreatedDateTime = product.CreatedDateTime,
                    UpdatedBy = product.UpdatedBy,
                    UpdatedDateTime = product.UpdatedDateTime,
                });
            }

            return await queryFactory.Query(TableName)
                .Where("Id", product.Id)
                .UpdateAsync(new
                {
                    Name = product.Name,
                    CatId = product.CatId,
                    SubCatId = product.SubCatId,
                    Description = product.Description,
                    Price = product.Price,
                    ImageName = product.ImageName,
                    IsActive = product.IsActive,
                    UpdatedBy = product.UpdatedBy,
                    UpdatedDateTime = product.UpdatedDateTime,
                });
        }
        public async Task<ProductInfo> GetByIdAsync(long id)
        {

            var result = await queryFactory.Query(TableName)
                 .Where("IsActive", true)
                .Where("Id", id)
                .GetAsync<ProductInfo>();

            return result.FirstOrDefault() ?? new ProductInfo();
        }

        public async Task<string> GetImageNameByIdAsync(long id)
        {

            var result = await queryFactory.Query(TableName)
                .Select("ImageName")
                .Where("IsActive", true)
                .Where("Id", id)
                .FirstOrDefaultAsync<string>();

            return result ?? string.Empty;
        }
        public async Task<List<ProductInfoDto>> GetAllProductsAsync()
        {
            try
            {
                var results = await queryFactory.Query(TableName)
                    .Join(CatTable, $"{CatTable}.Id", $"{TableName}.CatId")
                    .Join(SubCatTable, $"{SubCatTable}.Id", $"{TableName}.SubCatId")
                    .Where($"{TableName}.IsActive", true)
                    .Where($"{CatTable}.IsActive", true)
                    .Where($"{SubCatTable}.IsActive", true)
                    .Select($"{TableName}.*", $"{CatTable}.Name as CatName", $"{SubCatTable}.Name as SubCatName")
                    .GetAsync<ProductInfoDto>();

                return results.ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public async Task<int> DeleteProductAsync(long id)
        {
            return await queryFactory.Query(TableName)
                .Where("Id", id)
                .UpdateAsync(new
                {
                    IsActive = false
                });
        }

        public async Task<List<ProductInfoDto>> GetProductByCatSubCat(ProductFilterDto product)
        {
            try
            {
                var query = queryFactory.Query(TableName)
                            .Join(CatTable, $"{CatTable}.Id", $"{TableName}.CatId")
                    .Join(SubCatTable, $"{SubCatTable}.Id", $"{TableName}.SubCatId");

                DomainHelper.DynamicallySetWhere(product, query, TableName);
                var result = await query.Select($"{TableName}.*", $"{CatTable}.Name as CatName", $"{SubCatTable}.Name as SubCatName")
                        .GetAsync<ProductInfoDto>();

                return result.ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
