using ProductSale.Lib.App.Models;
using ProductSale.Lib.Domain;

namespace ProductSale.Lib.App.Builder
{
    public class SubCategoryDtoMapping
    {
        protected SubCategoryDtoMapping() { }

        public static List<SubCategoryDto> SetSubCategory(IEnumerable<SubCategory> subCategories)
        {
            return subCategories.Select(MapSetSubCategoryDomainToDto).ToList();
        }
        public static SubCategoryDto SetSubCategory(SubCategory subCategories)
        {
            return MapSetSubCategoryDomainToDto(subCategories);
        }
        private static SubCategoryDto MapSetSubCategoryDomainToDto(SubCategory subCategories)
        {
            if (subCategories == null) return new SubCategoryDto();

            return new SubCategoryDto
            {
                Id = subCategories.Id,
                Name = subCategories.Name,
                CatId = subCategories.CatId,
                IsActive = subCategories.IsActive,
            };
        }
    }
}
