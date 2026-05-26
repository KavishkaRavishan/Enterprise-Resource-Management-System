namespace ERMS.Application.Interfaces
{
    public interface IFileStorageService
    {
        Task<string> SaveFileAsync(Stream fileStream, string fileName, string folderName);
        void DeleteFile(string filePath);
    }
}
