namespace ProductSale.Lib.App.Models
{
    public class SubCategoryRequest
    {
        public long Id { get; set; }
        public string? Name { get; set; }
        public long CatId { get; set; }
    }
}
