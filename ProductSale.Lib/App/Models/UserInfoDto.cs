namespace ProductSale.Lib.App.Models
{
    public class UserInfoDto
    {
        public long Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string EmailId { get; set; } = string.Empty;
        public bool EmailSend { get; set; } = false;
    }
}
