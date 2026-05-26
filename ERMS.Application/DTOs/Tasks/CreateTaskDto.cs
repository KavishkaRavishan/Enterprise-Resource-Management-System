namespace ERMS.Application.DTOs.Tasks
{
    public class CreateTaskDto
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Priority { get; set; } = "Medium";
        public DateTime? DueDate { get; set; }
        public Guid ProjectId { get; set; }
        public Guid? AssignedToId { get; set; }
    }
}
