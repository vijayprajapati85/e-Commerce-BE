using ProductSale.Lib.App.Models;
using ProductSale.Lib.Domain;

namespace ProductSale.Lib.Infra.Repo
{
    public interface ICategoryRepository
    {
        Task<int> UpsertCateogryAsync(Category category);
        Task<Category> GetByIdAsync(long id);
        Task<List<Category>> GetAllCategoryAsync();
        Task<int> DeleteCategoryAsync(DeleteRequest deleteRequest);

    }
}
