using ProductSale.Lib.App.Models;
using ProductSale.Lib.Domain;

namespace ProductSale.Lib.App.Builder
{
    public class CategoryDtoMapping
    {
        protected CategoryDtoMapping() { }

        public static List<CategoryDto> SetCategory(IEnumerable<Category> categories)
        {
            return categories.Select(MapCategoryDomainToDto).ToList();
        }
        public static CategoryDto SetCategory(Category category)
        {
            return MapCategoryDomainToDto(category);
        }
        private static CategoryDto MapCategoryDomainToDto(Category category)
        {
            if (category == null) return new CategoryDto();

            return new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                IsActive = category.IsActive,
                SubCategories = category.SubCategories != null ? SubCategoryDtoMapping.SetSubCategory(category.SubCategories) : null,
            };
        }
    }
}
