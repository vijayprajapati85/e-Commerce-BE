namespace ProductSale.Lib.App.Models.Email
{
    public class EmailMessage
    {
        public string? SenderName { get; set; }
        public EmailAddress Sender { get; set; } = new();
        public ICollection<EmailAddress> Recipients { get; set; } = [];
        public string Subject { get; set; } = string.Empty;
        public string? Body { get; set; }
        public string? TemplateFile { get; set; }
        public Dictionary<string, object> Data { get; set; } = [];
    }
}
