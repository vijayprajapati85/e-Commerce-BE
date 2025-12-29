using Microsoft.AspNetCore.Http;

namespace ProductSale.Lib.App.Utility
{
    public interface IFileHelper
    {
        string GetFileAsString(string path);
        byte[] ConvertFileToBytes(IFormFile file);
        string GetFileExtension(string fileName);
        string GetUniqueFileName(string fileName);
    }
}
