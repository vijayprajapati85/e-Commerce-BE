using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using ProductSale.Lib.App.Builder;
using ProductSale.Lib.App.Constants;
using ProductSale.Lib.App.Models;
using ProductSale.Lib.Domain;
using ProductSale.Lib.Infra.Repo;

namespace ProductSale.Lib.App.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _repository;
        private readonly ILogger<CategoryService> _logger;
        private readonly IMemoryCache _cache;
        public string? UserId { get; set; }
        public CategoryService(ICategoryRepository repository, ILogger<CategoryService> logger, IMemoryCache cache)
        {
            _repository = repository;
            _logger = logger;
            _cache = cache;
        }

        public async Task<List<CategoryDto>?> GetAllCategoryAsync()
        {
            try
            {
                _logger.LogInformation("Inside GetAllCategoryAsync ===");
                if (_cache.TryGetValue(CacheKey.AllCategory, out List<CategoryDto>? categoriesDto))
                {
                    _logger.LogInformation("Fetching categories from cache.");
                    return categoriesDto; // Return cached data if found
                }

                List<Category> categories = await _repository.GetAllCategoryAsync();
                var result = CategoryDtoMapping.SetCategory(categories);
                _cache.Set(CacheKey.AllCategory, result);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError("Inside GetAllCategoryAsync === {error}", ex);
                return null;
            }
        }

        public async Task<CategoryDto?> GetByIdAsync(long id)
        {
            try
            {
                _logger.LogInformation("Inside GetByIdAsync ===");

                if (_cache.TryGetValue(CacheKey.CatIdKey(id), out CategoryDto? categoryDto))
                {
                    return categoryDto;
                }

                Category category = await _repository.GetByIdAsync(id);
                var result = CategoryDtoMapping.SetCategory(category);
                _cache.Set(CacheKey.CatIdKey(id), result);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError("Inside GetByIdAsync === {error}", ex);
                return null;
            }
        }

        public async Task<int> UpsertCateogryAsync(CategoryRequest category)
        {
            try
            {
                _logger.LogInformation("Inside UpsertCateogryAsync ===");
                _cache.Remove(CacheKey.AllCategory);
                if (category.Id != 0)
                {
                    _cache.Remove(CacheKey.ProductByCatSubCatKey(category.Id, null));
                    if (_cache.TryGetValue(CacheKey.CatIdKey(category.Id), out CategoryDto? dto) && dto?.SubCategories != null)
                    {
                        dto.SubCategories.ForEach(sub => _cache.Remove(CacheKey.ProductByCatSubCatKey(category.Id, sub.Id)));
                    }
                    _cache.Remove(CacheKey.CatIdKey(category.Id));
                }
                
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
                _logger.LogError("Error: UpsertCateogryAsync => {error}", ex);
                return 0;
            }
        }

        public async Task<int> DeleteCategoryAsync(long id)
        {
            try
            {
                _logger.LogInformation("Inside DeleteCategoryAsync ===");
                _cache.Remove(CacheKey.AllCategory);
                _cache.Remove(CacheKey.ProductByCatSubCatKey(id, null));
                if (_cache.TryGetValue(CacheKey.CatIdKey(id), out CategoryDto? dto) && dto?.SubCategories != null)
                {
                    dto.SubCategories.ForEach(sub => _cache.Remove(CacheKey.ProductByCatSubCatKey(id, sub.Id)));
                }
                _cache.Remove(CacheKey.CatIdKey(id));
                
                return await _repository.DeleteCategoryAsync(new DeleteRequest
                {
                    Id = id,
                    UpdatedBy = UserId ?? string.Empty,
                });
            }
            catch (Exception ex)
            {
                _logger.LogError("Error: UpsertCateogryAsync => {error}", ex);
                return 0;
            }
        }
    }
}
