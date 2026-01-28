namespace ProductSale.Lib.App.Models.Email
{
    public class ContactInfo
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public DateTime CreatedDateTime { get; set; }
    }
}
