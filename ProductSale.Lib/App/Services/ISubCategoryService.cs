using ProductSale.Lib.App.Models;

namespace ProductSale.Lib.App.Services
{
    public interface ISubCategoryService
    {
        string? UserId { get; set; }
        Task<int> UpsertSubCateogryAsync(SubCategoryRequest subCategory);
        Task<SubCategoryDto?> GetByIdAsync(long id);
        Task<List<SubCategoryDto>?> GetByCatIdAsync(long id);
        Task<List<SubCategoryDto>?> GetAllSubCategoryAsync();
        Task<int> DeleteSubCategoryAsync(long id);
    }
}
