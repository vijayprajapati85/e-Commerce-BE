using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using ProductSale.Lib.App.Builder;
using ProductSale.Lib.App.Constants;
using ProductSale.Lib.App.Models;
using ProductSale.Lib.Domain;
using ProductSale.Lib.Infra.Repo;

namespace ProductSale.Lib.App.Services
{
    public class SubCategoryService : ISubCategoryService
    {
        private readonly ISubCategoryRepository _repository;
        private readonly IMemoryCache _cache;
        private readonly ILogger<SubCategoryService> _logger;
        public string? UserId { get; set; }
        public SubCategoryService(ISubCategoryRepository repository, ILogger<SubCategoryService> logger, IMemoryCache cache)
        {
            _repository = repository;
            _logger = logger;
            _cache = cache;
        }

        public async Task<List<SubCategoryDto>?> GetAllSubCategoryAsync()
        {
            try
            {
                _logger.LogInformation("Inside GetAllSubCategoryAsync ===");
                if(_cache.TryGetValue(CacheKey.AllSubCategory, out List<SubCategoryDto> ? subCategoryDtos))
                {
                    return subCategoryDtos;
                }

                List<SubCategory> categories = await _repository.GetAllSubCategoryAsync();
                var result = SubCategoryDtoMapping.SetSubCategory(categories);
                _cache.Set(CacheKey.AllSubCategory, result);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError("Inside GetAllSubCategoryAsync === {error}", ex);
                return null;
            }
        }

        public async Task<SubCategoryDto?> GetByIdAsync(long id)
        {
            try
            {
                _logger.LogInformation("Inside GetByIdAsync ===");
                if(_cache.TryGetValue(CacheKey.SubIdKey(id), out SubCategoryDto? subCategoryDto))
                {
                    return subCategoryDto;
                }

                SubCategory category = await _repository.GetByIdAsync(id);
                var result = SubCategoryDtoMapping.SetSubCategory(category);
                _cache.Set(CacheKey.SubIdKey(id), result);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError("Inside GetByIdAsync === {error}", ex);
                return null;
            }
        }

        public async Task<List<SubCategoryDto>?> GetByCatIdAsync(long id)
        {
            try
            {
                _logger.LogInformation("Inside GetByCatIdAsync ===");

                if (_cache.TryGetValue(CacheKey.SubCatByCatId(id), out List<SubCategoryDto>? subCategoryDtos))
                {
                    return subCategoryDtos;
                }
                List<SubCategory> categories = await _repository.GetByCatIdAsync(id);
                var result = SubCategoryDtoMapping.SetSubCategory(categories);
                _cache.Set(CacheKey.SubCatByCatId(id), result);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError("Inside GetByCatIdAsync === {error}", ex);
                return null;
            }
        }

        public async Task<int> UpsertSubCateogryAsync(SubCategoryRequest subCategory)
        {
            try
            {
                _logger.LogInformation("Inside UpsertSubCateogryAsync ===");

                _cache.Remove(CacheKey.AllCategory);
                _cache.Remove(CacheKey.AllSubCategory);
                if (subCategory.Id != 0)
                {
                    _cache.Remove(CacheKey.SubIdKey(subCategory.Id));
                    _cache.Remove(CacheKey.SubCatByCatId(subCategory.CatId));
                }

                return await _repository.UpsertSubCateogryAsync(new SubCategory
                {
                    Id = subCategory.Id,
                    Name = subCategory.Name,
                    CatId = subCategory.CatId,
                    IsActive = true,
                    CreatedBy = UserId,
                    CreatedDateTime = DateTime.Now,
                    UpdatedBy = UserId,
                    UpdatedDateTime = DateTime.Now,
                });
            }
            catch (Exception ex)
            {
                _logger.LogError("Inside UpsertSubCateogryAsync === {error}", ex);
                return 0;
            }
        }

        public async Task<int> DeleteSubCategoryAsync(long id)
        {
            try
            {
                _logger.LogInformation("Inside DeleteSubCategoryAsync ===");

                _cache.Remove(CacheKey.AllCategory);
                _cache.Remove(CacheKey.AllSubCategory);
                _cache.Remove(CacheKey.SubIdKey(id));

                var result = await _repository.GetByIdAsync(id);
                if (result != null)
                {
                    _cache.Remove(CacheKey.SubCatByCatId(result.CatId));
                }

                return await _repository.DeleteSubCategoryAsync(new DeleteRequest
                {
                    Id = id,
                    UpdatedBy = UserId ?? string.Empty,
                });
            }
            catch (Exception ex)
            {
                _logger.LogError("Inside DeleteSubCategoryAsync === {error}",ex);
                return 0;
            }
        }

    }
}
