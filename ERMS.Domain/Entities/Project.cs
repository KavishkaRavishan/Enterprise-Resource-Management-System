using ERMS.Domain.Enums;

namespace ERMS.Domain.Entities
{
    public class Project : BaseEntity
    {
        public string Name { get; private set; } = string.Empty;
        public string Description { get; private set; } = string.Empty;
        public DateTime StartDate { get; private set; }
        public DateTime? EndDate { get; private set; }
        public ProjectStatus Status { get; private set; } = ProjectStatus.NotStarted;

        // Foreign key
        public Guid CreatedById { get; private set; }

        // Navigation properties
        public User CreatedBy { get; private set; } = null!;
        public ICollection<ProjectMember> Members { get; private set; } = new List<ProjectMember>();
        public ICollection<ProjectTask> Tasks { get; private set; } = new List<ProjectTask>();

        private Project() { } // For EF Core

        public Project(string name, string description, DateTime startDate, DateTime? endDate, Guid createdById)
        {
            Name = name;
            Description = description;
            StartDate = startDate;
            EndDate = endDate;
            CreatedById = createdById;
        }

        public void UpdateDetails(string name, string description, DateTime startDate, DateTime? endDate)
        {
            Name = name;
            Description = description;
            StartDate = startDate;
            EndDate = endDate;
            Updated = DateTime.UtcNow;
        }

        public void UpdateStatus(ProjectStatus status)
        {
            Status = status;
            Updated = DateTime.UtcNow;
        }
    }
}
