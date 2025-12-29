namespace ProductSale.Lib.App.Models
{
    public class CategoryDto
    {
        public long Id { get; set; }
        public string? Name { get; set; }
        public bool IsActive { get; set; }
        public List<SubCategoryDto>? SubCategories { get; set; }
    }
}
