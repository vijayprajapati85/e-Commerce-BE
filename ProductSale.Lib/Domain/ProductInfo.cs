using ProductSale.Lib.Domain;

namespace ProductSale.Domain
{
    public class ProductInfo : IAuditEntity
    {
        public long Id { get; set; }
        public string? Name { get; set; }
        public long CatId { get; set; }
        public long SubCatId { get; set; }
        public string Description { get; set; } = string.Empty;
        public string ImageName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public bool IsActive { get; set; }
        public string? CreatedBy { get; set; }
        public DateTimeOffset CreatedDateTime { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTimeOffset UpdatedDateTime { get; set; }
    }
}
