namespace ERMS.Application.DTOs.TimeLogs
{
    public class TimeLogDto
    {
        public Guid Id { get; set; }
        public Guid TaskId { get; set; }
        public string TaskTitle { get; set; } = string.Empty;
        public Guid UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string UserAvatar { get; set; } = string.Empty;
        public decimal HoursSpent { get; set; }
        public string Description { get; set; } = string.Empty;
        public DateTime DateLogged { get; set; }
        public DateTime Created { get; set; }
    }
}
