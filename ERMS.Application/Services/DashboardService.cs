using ERMS.Application.Common;
using ERMS.Application.DTOs.Dashboard;
using ERMS.Application.Interfaces;
using ERMS.Application.Interfaces.Repositories;
using ERMS.Domain.Enums;

namespace ERMS.Application.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly IUserRepository _userRepository;
        private readonly IProjectRepository _projectRepository;
        private readonly ITaskRepository _taskRepository;

        public DashboardService(
            IUserRepository userRepository,
            IProjectRepository projectRepository,
            ITaskRepository taskRepository)
        {
            _userRepository = userRepository;
            _projectRepository = projectRepository;
            _taskRepository = taskRepository;
        }

        public async Task<ServiceResult<DashboardDto>> GetDashboardAsync()
        {
            var totalUsers = await _userRepository.GetCountAsync();
            var activeUsers = await _userRepository.GetActiveCountAsync();
            var totalProjects = await _projectRepository.GetCountAsync();
            var activeProjects = await _projectRepository.GetActiveCountAsync();
            var totalTasks = await _taskRepository.GetCountAsync();
            var pendingTasks = await _taskRepository.GetCountByStatusAsync(TaskItemStatus.ToDo);
            var inProgressTasks = await _taskRepository.GetCountByStatusAsync(TaskItemStatus.InProgress);
            var completedTasks = await _taskRepository.GetCountByStatusAsync(TaskItemStatus.Done);

            // Get project summaries
            var projects = await _projectRepository.GetAllAsync();
            var projectSummaries = projects.Select(p => new ProjectSummaryDto
            {
                Id = p.Id,
                Name = p.Name,
                Status = p.Status.ToString(),
                TotalTasks = p.Tasks?.Count ?? 0,
                CompletedTasks = p.Tasks?.Count(t => t.Status == TaskItemStatus.Done) ?? 0,
                ProgressPercentage = p.Tasks != null && p.Tasks.Count > 0
                    ? Math.Round((double)p.Tasks.Count(t => t.Status == TaskItemStatus.Done) / p.Tasks.Count * 100, 1)
                    : 0,
                TotalHoursLogged = p.Tasks?.SelectMany(t => t.TimeLogs).Sum(tl => (double)tl.HoursSpent) ?? 0
            }).ToList();

            var dashboard = new DashboardDto
            {
                TotalUsers = totalUsers,
                ActiveUsers = activeUsers,
                TotalProjects = totalProjects,
                ActiveProjects = activeProjects,
                TotalTasks = totalTasks,
                PendingTasks = pendingTasks,
                InProgressTasks = inProgressTasks,
                CompletedTasks = completedTasks,
                ProjectSummaries = projectSummaries,
                RecentActivities = new List<RecentActivityDto>() // Simplified — no activity tracking entity
            };

            return ServiceResult<DashboardDto>.Ok(dashboard);
        }
    }
}
