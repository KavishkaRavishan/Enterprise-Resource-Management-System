namespace ERMS.Domain.Entities
{
    public class Attachment : BaseEntity
    {
        public string FileName { get; private set; } = string.Empty;
        public long Size { get; private set; }
        public string FilePath { get; private set; } = string.Empty;
        public string ContentType { get; private set; } = string.Empty;

        // Foreign key
        public Guid TaskId { get; private set; }
        public ProjectTask Task { get; private set; } = null!;

        // Track who uploaded it
        public Guid UploadedById { get; private set; }
        public User UploadedBy { get; private set; } = null!;

        private Attachment() { } // For EF Core

        public Attachment(string fileName, long size, string filePath, string contentType, Guid taskId, Guid uploadedById)
        {
            FileName = fileName;
            Size = size;
            FilePath = filePath;
            ContentType = contentType;
            TaskId = taskId;
            UploadedById = uploadedById;
        }
    }
}
