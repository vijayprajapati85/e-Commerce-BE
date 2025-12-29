using ProductSale.Domain;
using ProductSale.Lib.App.Models;

namespace ProductSale.Lib.Infra.Repo
{
	public interface IProductRepository
	{
		Task<int> UpsertProductAsync(ProductInfo product);
		Task<ProductInfo> GetByIdAsync(long id);
		Task<List<ProductInfoDto>> GetAllProductsAsync();
		Task<int> DeleteProductAsync(long id);
		Task<string> GetImageNameByIdAsync(long id);
		Task<List<ProductInfoDto>> GetProductByCatSubCat(ProductFilterDto product);

    }
}
