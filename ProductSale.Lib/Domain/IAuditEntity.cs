namespace ProductSale.Lib.Domain
{
    public interface IAuditEntity
    {
        string? CreatedBy { get; set; }
        DateTimeOffset CreatedDateTime { get; set; }

        string? UpdatedBy { get; set; }
        DateTimeOffset UpdatedDateTime { get; set; }
    }
}
