using Microsoft.Extensions.Logging;
using ProductSale.Lib.App.Builder;
using ProductSale.Lib.App.Models;
using ProductSale.Lib.Domain;
using ProductSale.Lib.Infra.Repo;

namespace ProductSale.Lib.App.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _repository;
        private readonly ILogger<CategoryService> _logger;
        public string? UserId { get; set; }
        public CategoryService(ICategoryRepository repository, ILogger<CategoryService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<List<CategoryDto>?> GetAllCategoryAsync()
        {
            try
            {
                _logger.LogInformation("Inside GetAllCategoryAsync ===");
                List<Category> categories = await _repository.GetAllCategoryAsync();
                return CategoryDtoMapping.SetCategory(categories);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error: GetAllCategoryAsync => {error}", ex.Message);
                return null;
            }
        }

        public async Task<CategoryDto?> GetByIdAsync(long id)
        {
            try
            {
                Category category = await _repository.GetByIdAsync(id);
                return CategoryDtoMapping.SetCategory(category);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public async Task<int> UpsertCateogryAsync(CategoryRequest category)
        {
            try
            {
                _logger.LogInformation("Inside UpsertCateogryAsync ===");
                return await _repository.UpsertCateogryAsync(new Category
                {
                    Id = category.Id,
                    Name = category.Name,
                    IsActive = true,
                    CreatedBy = UserId,
                    CreatedDateTime = DateTime.Now,
                    UpdatedBy = UserId,
                    UpdatedDateTime = DateTime.Now,
                });
            }
            catch (Exception ex)
            {
                _logger.LogError("Error: UpsertCateogryAsync => {error}", ex.Message);
                return 0;
            }
        }

        public async Task<int> DeleteCategoryAsync(long id)
        {
            try
            {
                return await _repository.DeleteCategoryAsync(new DeleteRequest
                {
                    Id = id,
                    UpdatedBy = UserId ?? string.Empty,
                });
            }
            catch (Exception)
            {
                throw;
                //Console.WriteLine(ex.Message);
                //return 0;
            }
        }
    }
}
