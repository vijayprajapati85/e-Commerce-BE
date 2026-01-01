using Microsoft.Extensions.Configuration;
using ProductSale.Domain;
using ProductSale.Lib.App.Builder;
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
        public string? UserId { get; set; }
        public ProductService(IProductRepository repository, IConfiguration configuration, IFileHelper fileHelper)
        {
            _repository = repository;
            _configuration = configuration;
            _fileHelper = fileHelper;
        }

        public async Task<List<ProductInfoDto>?> GetAllProductsAsync(string folderPath)
        {
            try
            {
                var result = await _repository.GetAllProductsAsync();
                result.ForEach(async result =>
                {
                    if (!string.IsNullOrEmpty(result.ImageName))
                    {
                        result.ImageName = folderPath + $"{result.ImageName}";
                    }
                });

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public async Task<ProductInfoDto?> GetByIdAsync(long id)
        {
            try
            {
                ProductInfo productInfo = await _repository.GetByIdAsync(id);
                return ProductInfoDtoMapping.SetProductInfo(productInfo);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public async Task<int> UpsertProductAsync(ProductInfoRequest product, string folderPath)
        {
            try
            {
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
                Console.WriteLine(ex.Message);
                return 0;
            }
        }

        public async Task<int> DeleteProductAsync(long id)
        {
            try
            {
                return await _repository.DeleteProductAsync(id);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return 0;
            }
        }
        public async Task<List<ProductInfoDto>> GetProductByCatSubCat(ProductFilterDto product, string folderPath)
        {
            try
            {
                var result = await _repository.GetProductByCatSubCat(product);

                if (result != null)
                {
                    result.ForEach(async result =>
                    {
                        if (!string.IsNullOrEmpty(result.ImageName))
                        {
                            result.ImageName = folderPath + $"{result.ImageName}";
                        }
                    });
                }
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

    }
}
