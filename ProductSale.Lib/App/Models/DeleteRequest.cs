namespace ProductSale.Lib.App.Models
{
    public class DeleteRequest
    {
        public long Id { get; set; }
        public string UpdatedBy { get; set; } = string.Empty;
    }
}
