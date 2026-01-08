using ProductSale.Lib.Domain;

namespace ProductSale.Lib.App.Constants
{
    public class CacheKey
    {
        public const string AllCategory = "AllCategory";
        public const string AllSubCategory = "AllSubCategory";


        public const string SubCategory = $"SubCategory:";
        public static string SubIdKey(long id)
        {
            return $"{SubCategory}{id}";
        }
        public static string SubCatByCatId(long catId)
        {
            return $"{SubCategory}{catId}";
        }

        public const string Category = $"Category:";
        public static string CatIdKey(long id)
        {
            return $"{Category}{id}";
        }

        public const string AllProduct = "AllProduct";
        public const string Product = $"Product:";
        public static string ProductIdKey(long id)
        {
            return $"{Product}{id}";
        }

        public const string ProductByCatSubCat = $"ProductByCatSubCat";
        public static string ProductByCatSubCatKey(long? catId, long? subId)
        {
            return $"{ProductByCatSubCat}:{catId}:{subId}";
        }
    }
}
