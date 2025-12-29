namespace ProductSale.Lib.Domain
{
    public class Category : IAuditEntity
    {
        public long Id { get; set; }
        public string? Name { get; set; }
        public bool IsActive { get; set; }
        public string? CreatedBy { get; set; }
        public DateTimeOffset CreatedDateTime { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTimeOffset UpdatedDateTime { get; set; }
        public List<SubCategory>? SubCategories { get; set; }
    }
}
