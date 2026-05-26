namespace ERMS.Application.DTOs.Projects
{
    public class ProjectDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public Guid CreatedById { get; set; }
        public string CreatedByName { get; set; } = string.Empty;
        public int MemberCount { get; set; }
        public int TaskCount { get; set; }
        public int CompletedTaskCount { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public List<ProjectMemberDto> Members { get; set; } = new();
    }

    public class ProjectMemberDto
    {
        public Guid UserId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? AvatarPath { get; set; }
        public DateTime JoinedAt { get; set; }
    }
}
