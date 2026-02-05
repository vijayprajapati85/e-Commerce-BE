namespace ProductSale.Lib.App.Models
{
    public class ProductFilterDto
    {
        public long? CatId { get; set; }
        public long? SubCatId { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
