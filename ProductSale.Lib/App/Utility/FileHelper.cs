using Microsoft.AspNetCore.Http;
namespace ProductSale.Lib.App.Utility
{
    public class FileHelper : IFileHelper
    {
        public string GetFileAsString(string path)
        {
            return File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), path));
        }

        public byte[] ConvertFileToBytes(IFormFile file)
        {
            MemoryStream memoryStream = new MemoryStream();
            using (Stream stream = file.OpenReadStream())
            {
                stream.CopyTo(memoryStream);
            }
            memoryStream.Close();
            return memoryStream.ToArray();
        }

        public string GetFileExtension(string fileName)
        {
            return Path.GetExtension(fileName);
        }

        public string GetUniqueFileName(string fileName)
        {
            return Guid.NewGuid().ToString() + Path.GetExtension(fileName);
        }
    }
}
