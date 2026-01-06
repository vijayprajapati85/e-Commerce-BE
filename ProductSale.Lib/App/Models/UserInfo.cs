namespace ProductSale.Lib.App.Models
{
    public class UserInfo
    {
        public long Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string EmailId { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public bool EmailSend { get; set; } = false;
        public DateTimeOffset CreatedDateTime { get; set; }
        public DateTimeOffset UpdatedDateTime { get; set; }
    }
}
