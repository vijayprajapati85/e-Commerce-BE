using ProductSale.Lib.App.Models;

namespace ProductSale.Lib.App.Services
{
    public interface ICategoryService
    {
        string? UserId { get; set; }
        Task<int> UpsertCateogryAsync(CategoryRequest category);
        Task<CategoryDto?> GetByIdAsync(long id);
        Task<List<CategoryDto>?> GetAllCategoryAsync();
        Task<int> DeleteCategoryAsync(long id);
    }
}
