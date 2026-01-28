namespace ProductSale.Lib.App.Models.Cart
{
    public class Order
    {
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public long Quantity { get; set; }
        public string OrderId { get; set; } = string.Empty;
        public long ProductId { get; set; }
        public string ImageName { get; set; } = string.Empty;

    }
}
