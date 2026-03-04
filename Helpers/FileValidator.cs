namespace RecipeProject.Helpers
{
    public class FileValidator
    {
        public static bool IsImage(IFormFile file)
        {
            return file.ContentType.StartsWith("image/");
        }

        public static bool IsValidSize(IFormFile file, int maxSizeMB = 5)
        {
            return file.Length <= maxSizeMB * 1024 * 1024;
        }

        public static async Task<string> SaveFileAsync(IFormFile file, string folder)
        {
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", folder);

            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return $"/uploads/{folder}/{fileName}";
        }

        public static void DeleteFile(string? fileUrl)
        {
            if (string.IsNullOrEmpty(fileUrl)) return;

            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", fileUrl.TrimStart('/'));

            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
    }
}
