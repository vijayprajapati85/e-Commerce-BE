using Microsoft.AspNetCore.Http;

namespace ProductSale.Lib.App.Models
{
    public class ProductInfoRequest
    {
        public long Id { get; set; }
        public string? Name { get; set; }
        public long CatId { get; set; }
        public long SubCatId { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public IFormFile? ImageFile { get; set; }
    }
}
