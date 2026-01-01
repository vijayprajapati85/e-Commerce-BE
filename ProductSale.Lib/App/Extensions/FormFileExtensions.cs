using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using ProductSale.Lib.App.Utility;

namespace ProductSale.Lib.App.Extensions
{
    public static class FormFileExtensions
    {
        public static void ValidateFileType(this IFormFile file, IConfiguration configuration, IFileHelper helper)
        {
            if (file == null) { return; }

            var fileExtension = helper.GetFileExtension(file.FileName);
            var allowedExtension = configuration.GetSection("FileExtensions").GetSection(fileExtension).Value;
            if (allowedExtension == null)
            {
                throw new Exception($"File type not allowed {fileExtension}");
            }
        }

        public static void ValidateFileSize(this IFormFile file)
        {
            const int fiveMb = 5 * 1024 * 1024;
            if (file.Length > fiveMb)
            {
                throw new Exception("File size more than 5 mb");
            }

        }

        public static async Task<string> UploadFileAsync(this IFormFile file, IFileHelper helper, string basePath)
        {
            if (file == null)
            {
                return string.Empty;
            }
            try
            {
                var uniqueName = helper.GetUniqueFileName(file.FileName);

                if (!Directory.Exists(basePath))
                {
                    Directory.CreateDirectory(basePath);
                }

                var filePath = Path.Combine(basePath, uniqueName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                return uniqueName;
            }
            catch (Exception)
            {

                return string.Empty;
            }
        }
    }
}
