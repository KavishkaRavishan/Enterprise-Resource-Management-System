using ERMS.Domain.Enums;

namespace ERMS.Domain.Entities
{
    public class ProjectTask : BaseEntity
    {
        public string Title { get; private set; } = string.Empty;
        public string Description { get; private set; } = string.Empty;
        public TaskItemStatus Status { get; private set; } = TaskItemStatus.ToDo;
        public TaskPriority Priority { get; private set; } = TaskPriority.Medium;
        public DateTime? DueDate { get; private set; }

        // Foreign keys
        public Guid ProjectId { get; private set; }
        public Guid? AssignedToId { get; private set; }

        // Navigation properties
        public Project Project { get; private set; } = null!;
        public User? AssignedTo { get; private set; }
        public ICollection<TaskComment> Comments { get; private set; } = new List<TaskComment>();
        public ICollection<TimeLog> TimeLogs { get; private set; } = new List<TimeLog>();
        public ICollection<Attachment> Attachments { get; private set; } = new List<Attachment>();

        private ProjectTask() { } // For EF Core

        public ProjectTask(string title, string description, Guid projectId, TaskPriority priority, DateTime? dueDate)
        {
            Title = title;
            Description = description;
            ProjectId = projectId;
            Priority = priority;
            DueDate = dueDate;
        }

        public void UpdateDetails(string title, string description, TaskPriority priority, DateTime? dueDate)
        {
            Title = title;
            Description = description;
            Priority = priority;
            DueDate = dueDate;
            Updated = DateTime.UtcNow;
        }

        public void UpdateStatus(TaskItemStatus status)
        {
            Status = status;
            Updated = DateTime.UtcNow;
        }

        public void Assign(Guid? userId)
        {
            AssignedToId = userId;
            Updated = DateTime.UtcNow;
        }
    }
}
