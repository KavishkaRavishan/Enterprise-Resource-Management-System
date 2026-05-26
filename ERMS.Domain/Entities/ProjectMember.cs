namespace ERMS.Domain.Entities
{
    public class ProjectMember
    {
        public Guid ProjectId { get; private set; }
        public Guid UserId { get; private set; }
        public DateTime JoinedAt { get; private set; }

        // Navigation properties
        public Project Project { get; private set; } = null!;
        public User User { get; private set; } = null!;

        private ProjectMember() { } // For EF Core

        public ProjectMember(Guid projectId, Guid userId)
        {
            ProjectId = projectId;
            UserId = userId;
            JoinedAt = DateTime.UtcNow;
        }
    }
}
