using ERMS.Application.Interfaces;

namespace ERMS.Infrastructure.Services
{
    public class FileStorageService : IFileStorageService
    {
        private readonly string _rootPath;

        public FileStorageService()
        {
            // Root path: inside ERMS.API/wwwroot
            _rootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
        }

        public async Task<string> SaveFileAsync(Stream fileStream, string fileName, string folderName)
        {
            if (fileStream == null || fileStream.Length == 0)
                throw new ArgumentException("File stream cannot be empty");

            // Ensure directory exists
            var targetFolder = Path.Combine(_rootPath, folderName);
            if (!Directory.Exists(targetFolder))
            {
                Directory.CreateDirectory(targetFolder);
            }

            // Create unique filename to avoid collision
            var uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(fileName)}";
            var filePath = Path.Combine(targetFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await fileStream.CopyToAsync(stream);
            }

            // Return relative path for web serving (e.g. /uploads/attachments/filename)
            return $"/{folderName}/{uniqueFileName}";
        }

        public void DeleteFile(string relativeFilePath)
        {
            if (string.IsNullOrEmpty(relativeFilePath)) return;

            // Convert web relative path back to physical absolute path
            var physicalPath = Path.Combine(_rootPath, relativeFilePath.TrimStart('/'));
            if (File.Exists(physicalPath))
            {
                File.Delete(physicalPath);
            }
        }
    }
}
