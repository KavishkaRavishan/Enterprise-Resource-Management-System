using ERMS.Domain.Entities;
using ERMS.Domain.Enums;

namespace ERMS.Application.Interfaces.Repositories
{
    public interface ITaskRepository
    {
        Task<ProjectTask?> GetByIdAsync(Guid id);
        Task<ProjectTask?> GetByIdWithDetailsAsync(Guid id);
        Task<List<ProjectTask>> GetByProjectIdAsync(Guid projectId);
        Task<List<ProjectTask>> GetByAssignedUserIdAsync(Guid userId);
        Task AddAsync(ProjectTask task);
        Task UpdateAsync(ProjectTask task);
        Task DeleteAsync(Guid id);
        Task<int> GetCountAsync();
        Task<int> GetCountByStatusAsync(TaskItemStatus status);
    }
}
