using ProductSale.Lib.App.Models;
using ProductSale.Lib.Domain;

namespace ProductSale.Lib.Infra.Repo
{
    public interface ISubCategoryRepository
    {
        Task<int> UpsertSubCateogryAsync(SubCategory subCategory);
        Task<SubCategory> GetByIdAsync(long id);
        Task<List<SubCategory>> GetByCatIdAsync(long id);
        Task<List<SubCategory>> GetAllSubCategoryAsync();
        Task<int> DeleteSubCategoryAsync(DeleteRequest deleteRequest);
    }
}
