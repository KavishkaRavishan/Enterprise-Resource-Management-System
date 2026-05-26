namespace ERMS.Application.DTOs.Attachments
{
    public class AttachmentDto
    {
        public Guid Id { get; set; }
        public string FileName { get; set; } = string.Empty;
        public long Size { get; set; }
        public string FilePath { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public Guid TaskId { get; set; }
        public Guid UploadedById { get; set; }
        public string UploadedByName { get; set; } = string.Empty;
        public DateTime Created { get; set; }
    }
}
