namespace ERMS.Application.DTOs.Notifications
{
    public class NotificationDto
    {
        public Guid Id { get; set; }
        public string Message { get; set; } = string.Empty;
        public bool IsRead { get; set; }
        public string Type { get; set; } = string.Empty;
        public Guid? ReferenceId { get; set; }
        public DateTime Created { get; set; }
    }
}
