using ProductSale.Domain;
using ProductSale.Lib.App.Models;

namespace ProductSale.Lib.App.Builder
{
    public class ProductInfoDtoMapping
    {
        protected ProductInfoDtoMapping() { }

        public static List<ProductInfoDto> SetProductsInfo(IEnumerable<ProductInfo> products)
        {
            return products.Select(MapProdcutInfoDomainToDto).ToList();
        }
        public static ProductInfoDto SetProductInfo(ProductInfo productInfo)
        {
            return MapProdcutInfoDomainToDto(productInfo);
        }
        private static ProductInfoDto MapProdcutInfoDomainToDto(ProductInfo productInfo)
        {
            if (productInfo == null) return new ProductInfoDto();

            return new ProductInfoDto
            {
                Id = productInfo.Id,
                Name = productInfo.Name,
                CatId = productInfo.CatId,
                SubCatId = productInfo.SubCatId,
                Description = productInfo.Description,
                IsActive = productInfo.IsActive,
                Price = productInfo.Price,
                ImageName = productInfo.ImageName,
            };
        }
    }
}
