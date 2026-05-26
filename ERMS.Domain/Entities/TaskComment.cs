namespace ERMS.Domain.Entities
{
    public class TaskComment : BaseEntity
    {
        public string Content { get; private set; } = string.Empty;

        // Foreign keys
        public Guid TaskId { get; private set; }
        public Guid AuthorId { get; private set; }

        // Navigation properties
        public ProjectTask Task { get; private set; } = null!;
        public User Author { get; private set; } = null!;

        private TaskComment() { } // For EF Core

        public TaskComment(string content, Guid taskId, Guid authorId)
        {
            Content = content;
            TaskId = taskId;
            AuthorId = authorId;
        }

        public void UpdateContent(string content)
        {
            Content = content;
            Updated = DateTime.UtcNow;
        }
    }
}
