namespace ProductSale.Lib.App.Models.Cart
{
    public class CartInfoDto
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public long ProductId { get; set; }
        public long Quantity { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTimeOffset CreatedDateTime { get; set; }
        public DateTimeOffset UpdatedDateTime { get; set; }
    }
}
