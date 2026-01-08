using Microsoft.AspNetCore.Http;

namespace ProductSale.Lib.App.Models
{
    public class ProductInfoDto
    {
        public long Id { get; set; }
        public string? Name { get; set; }
        public long CatId { get; set; }
        public string CatName { get; set; } = string.Empty;
        public long SubCatId { get; set; }
        public string SubCatName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string ImageName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public IFormFile? Image { get; set; }
    }
}
