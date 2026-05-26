namespace ERMS.Domain.Entities
{
    public class TimeLog : BaseEntity
    {
        public Guid TaskId { get; private set; }
        public ProjectTask Task { get; private set; } = null!;
        public Guid UserId { get; private set; }
        public User User { get; private set; } = null!;
        public decimal HoursSpent { get; private set; }
        public string Description { get; private set; } = string.Empty;
        public DateTime DateLogged { get; private set; }

        private TimeLog() { } // EF constructor

        public TimeLog(Guid taskId, Guid userId, decimal hoursSpent, string description, DateTime dateLogged)
        {
            TaskId = taskId;
            UserId = userId;
            HoursSpent = hoursSpent;
            Description = description;
            DateLogged = dateLogged;
        }

        public void UpdateDetails(decimal hoursSpent, string description, DateTime dateLogged)
        {
            HoursSpent = hoursSpent;
            Description = description;
            DateLogged = dateLogged;
            Updated = DateTime.UtcNow;
        }
    }
}
