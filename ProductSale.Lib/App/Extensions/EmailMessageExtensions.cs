using ProductSale.Lib.App.Models.Email;
using System.Dynamic;

namespace ProductSale.Lib.App.Extensions
{
    internal static partial class EmailMessageExtensions
    {
        public static dynamic TemplateData(this EmailMessage message)
        {
            ExpandoObject expandeObj = new ExpandoObject();
            var collection =  expandeObj as ICollection<KeyValuePair<string, object>>;

            foreach (var item in message.Data)
            {
                collection.Add(item);
            }

            dynamic templateData = collection;

            return templateData;
        }
        
    }
}
