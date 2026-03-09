namespace ProductSale.Lib.App.Models
{
    public class Profile
    {
        public long Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string MobileNo { get; set; } = string.Empty;
    }
}
