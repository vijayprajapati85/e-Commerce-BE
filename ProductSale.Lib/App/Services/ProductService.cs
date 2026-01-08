using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ProductSale.Domain;
using ProductSale.Lib.App.Builder;
using ProductSale.Lib.App.Constants;
using ProductSale.Lib.App.Extensions;
using ProductSale.Lib.App.Models;
using ProductSale.Lib.App.Utility;
using ProductSale.Lib.Infra.Repo;

namespace ProductSale.Lib.App.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _repository;
        private readonly IConfiguration _configuration;
        private readonly IFileHelper _fileHelper;
        private readonly IMemoryCache _cache;
        private readonly ILogger<ProductService> _logger;
        public string? UserId { get; set; }
        public ProductService(IProductRepository repository, IConfiguration configuration, IFileHelper fileHelper, IMemoryCache cache, ILogger<ProductService> logger)
        {
            _repository = repository;
            _configuration = configuration;
            _fileHelper = fileHelper;
            _cache = cache;
            _logger = logger;
        }

        public async Task<List<ProductInfoDto>?> GetAllProductsAsync(string folderPath)
        {
            try
            {
                _logger.LogInformation("Inside GetAllProductsAsync ===");
                if (_cache.TryGetValue(CacheKey.AllProduct, out List<ProductInfoDto>? productInfoDtos))
                {
                    return productInfoDtos;
                }

                var result = await _repository.GetAllProductsAsync();
                if (result != null && result.Count != 0)
                {
                    result.ForEach(result =>
                    {
                        if (!string.IsNullOrEmpty(result.ImageName))
                        {
                            result.ImageName = folderPath + $"{result.ImageName}";
                        }
                    });
                }
                _cache.Set(CacheKey.AllProduct, result);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError("Inside GetAllProductsAsync === {error}", ex);
                return null;
            }
        }

        public async Task<ProductInfoDto?> GetByIdAsync(long id)
        {
            try
            {
                _logger.LogInformation("Inside GetAllProductsAsync ===");
                if (_cache.TryGetValue(CacheKey.ProductIdKey(id), out ProductInfoDto? productInfoDto))
                {
                    return productInfoDto;
                }

                ProductInfo productInfo = await _repository.GetByIdAsync(id);
                var result = ProductInfoDtoMapping.SetProductInfo(productInfo);
                _cache.Set(CacheKey.ProductIdKey(id), result);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError("Inside GetByIdAsync === {error}", ex);
                return null;
            }
        }

        public async Task<List<ProductInfoDto>?> GetProductByCatSubCat(ProductFilterDto product, string folderPath)
        {
            try
            {
                _logger.LogInformation("Inside GetProductByCatSubCat ===");

                if (_cache.TryGetValue(CacheKey.ProductByCatSubCatKey(product.CatId, product.SubCatId), out List<ProductInfoDto>? productInfoDtos))
                {
                    return productInfoDtos;
                }

                var result = await _repository.GetProductByCatSubCat(product);

                if (result != null)
                {
                    result.ForEach(result =>
                    {
                        if (!string.IsNullOrEmpty(result.ImageName))
                        {
                            result.ImageName = folderPath + $"{result.ImageName}";
                        }
                    });
                }

                _cache.Set(CacheKey.ProductByCatSubCatKey(product.CatId, product.SubCatId), result);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError("Inside GetProductByCatSubCat === {error}", ex);
                return null;
            }
        }

        public async Task<int> UpsertProductAsync(ProductInfoRequest product, string folderPath)
        {
            try
            {
                _logger.LogInformation("Inside UpsertProductAsync ===");

                _cache.Remove(CacheKey.AllProduct);
                if (product.Id != 0)
                {
                    _cache.Remove(CacheKey.ProductIdKey(product.Id));
                    _cache.Remove(CacheKey.ProductByCatSubCatKey(product.CatId, product.SubCatId));
                    _cache.Remove(CacheKey.ProductByCatSubCatKey(product.CatId, null));
                }

                var imageName = string.Empty;
                if (product.ImageFile != null)
                {
                    product.ImageFile.ValidateFileSize();
                    product.ImageFile.ValidateFileType(_configuration, _fileHelper);
                    imageName = await product.ImageFile.UploadFileAsync(_fileHelper, folderPath);
                }
                else if (product.Id > 0)
                {
                    imageName = await _repository.GetImageNameByIdAsync(product.Id);
                }

                ProductInfo productInfo = new ProductInfo
                {
                    Id = product.Id,
                    Name = product.Name,
                    CatId = product.CatId,
                    SubCatId = product.SubCatId,
                    Description = product.Description ?? string.Empty,
                    Price = product.Price,
                    ImageName = imageName,
                    IsActive = true,
                    CreatedBy = UserId,
                    CreatedDateTime = DateTime.Now,
                    UpdatedBy = UserId,
                    UpdatedDateTime = DateTime.Now,
                };

                return await _repository.UpsertProductAsync(productInfo);
            }
            catch (Exception ex)
            {
                _logger.LogError("Inside UpsertProductAsync === {error}", ex);
                return 0;
            }
        }

        public async Task<int> DeleteProductAsync(long id)
        {
            try
            {
                _logger.LogInformation("Inside DeleteProductAsync ===");

                _cache.Remove(CacheKey.AllProduct);
                _cache.Remove(CacheKey.ProductIdKey(id));

                var result = await _repository.GetByIdAsync(id);
                if (result != null)
                {
                    _cache.Remove(CacheKey.ProductByCatSubCatKey(result.CatId, result.SubCatId));
                    _cache.Remove(CacheKey.ProductByCatSubCatKey(result.CatId, null));
                }

                return await _repository.DeleteProductAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError("Inside DeleteProductAsync === {error}", ex);
                return 0;
            }
        }
       
    }
}
