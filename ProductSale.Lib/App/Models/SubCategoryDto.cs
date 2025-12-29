namespace ProductSale.Lib.App.Models
{
    public class SubCategoryDto
    {
        public long Id { get; set; }
        public string? Name { get; set; }
        public long CatId { get; set; }
        public bool IsActive { get; set; }
    }
}
