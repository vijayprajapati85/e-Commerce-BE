namespace ProductSale.Lib.App.Exceptions
{
    public class BusinessRuleException : Exception
    {
        public string Title { get; set; }
        public BusinessRuleException(string message) : base(message) { }

        public BusinessRuleException(string title, string message) : base(message)
        {
            Title = title;
        }
    }
}
