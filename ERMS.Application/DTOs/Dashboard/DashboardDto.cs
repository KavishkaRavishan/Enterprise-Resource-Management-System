namespace ERMS.Application.DTOs.Dashboard
{
    public class DashboardDto
    {
        public int TotalUsers { get; set; }
        public int ActiveUsers { get; set; }
        public int TotalProjects { get; set; }
        public int ActiveProjects { get; set; }
        public int TotalTasks { get; set; }
        public int PendingTasks { get; set; }
        public int InProgressTasks { get; set; }
        public int CompletedTasks { get; set; }
        public List<RecentActivityDto> RecentActivities { get; set; } = new();
        public List<ProjectSummaryDto> ProjectSummaries { get; set; } = new();
    }

    public class RecentActivityDto
    {
        public string Description { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
    }

    public class ProjectSummaryDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public int TotalTasks { get; set; }
        public int CompletedTasks { get; set; }
        public double ProgressPercentage { get; set; }
        public double TotalHoursLogged { get; set; }
    }
}
