namespace ProductSale.Lib.App.Extensions
{
    public static class DeepCopy
    {
        public static T? DeepCopyData<T>(this T source)
        {
            var jsonString = System.Text.Json.JsonSerializer.Serialize(source);
            return System.Text.Json.JsonSerializer.Deserialize<T>(jsonString);
        }
    }
}
