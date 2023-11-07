namespace BizNews.Helper
{
    public static class FileHelper
    {
        public static async Task<string> SaveFileAsync(this IFormFile file, string WebRootPath)
        {
            var filePath = Path.Combine(WebRootPath, "uploads").ToLower();
            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }
            var path = "/uploads/" + Guid.NewGuid().ToString() + file.FileName;
            using FileStream fileStream = new(WebRootPath + path, FileMode.Create);
            await file.CopyToAsync(fileStream);
            return path;
        }
    }
}
