namespace ProductSale.Lib.App.Models.Email
{
    public class EmailCommand
    {
        public string EmailType { get; set; } = string.Empty;
        public Dictionary<string, string> EmailData { get; set; } = new Dictionary<string, string>();
    }
}
