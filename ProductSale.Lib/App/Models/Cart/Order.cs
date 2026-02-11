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
        public string? OrderDate { get; set; } = string.Empty;

    }

    public class OrderData
    {
        public string OrderId { get; set; } = string.Empty;
        public string? OrderDate { get; set; } = string.Empty;
        public List<Order> Orders { get; set; } = new List<Order>();
    }

    public class TrackOrder
    {
        public List<OrderData> InProgress { get; set; } = new List<OrderData>();
        public List<OrderData> Completed { get; set; } = new List<OrderData>();
        public List<OrderData> Pending { get; set; } = new List<OrderData>();
    }
}
