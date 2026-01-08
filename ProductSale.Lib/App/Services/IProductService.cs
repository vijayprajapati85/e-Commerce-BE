using ProductSale.Lib.App.Models;

namespace ProductSale.Lib.App.Services
{
    public interface IProductService
    {
        string? UserId { get; set; }
        Task<int> UpsertProductAsync(ProductInfoRequest product, string folderPath);
        Task<ProductInfoDto?> GetByIdAsync(long id);
        Task<List<ProductInfoDto>?> GetAllProductsAsync(string folderPath);
        Task<int> DeleteProductAsync(long id);
        Task<List<ProductInfoDto>?> GetProductByCatSubCat(ProductFilterDto product, string folderPath);

    }
}
