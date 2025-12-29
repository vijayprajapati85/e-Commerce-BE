using SqlKata;
using System.Data;

namespace ProductSale.Lib.App.Extensions
{
    public static class DomainHelper
    {
        public static void DynamicallySetWhere<T>(T filterDto, Query query, string tableName = null)
        {
            var properties = typeof(T).GetProperties();
            foreach (var prop in properties)
            {
                var value = prop.GetValue(filterDto);
                if (value != null)
                {
                    if (prop.PropertyType == typeof(string))
                    {
                        query.WhereRaw(string.Format("LOWER({0}) = LOWER(?)", prop.Name.ToLower()), prop.GetValue(filterDto));
                    }
                    else
                    {
                        query.Where($"{tableName}.{prop.Name.ToLower()}", prop.GetValue(filterDto));
                    }
                }

            }
        }
    }
}
