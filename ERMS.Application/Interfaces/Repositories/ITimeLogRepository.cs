using ERMS.Domain.Entities;

namespace ERMS.Application.Interfaces.Repositories
{
    public interface ITimeLogRepository
    {
        Task<TimeLog?> GetByIdAsync(Guid id);
        Task<List<TimeLog>> GetByTaskIdAsync(Guid taskId);
        Task<List<TimeLog>> GetByUserIdAsync(Guid userId);
        Task<List<TimeLog>> GetByProjectIdAsync(Guid projectId);
        Task AddAsync(TimeLog timeLog);
        Task UpdateAsync(TimeLog timeLog);
        Task DeleteAsync(Guid id);
        Task<decimal> GetTotalHoursByProjectAsync(Guid projectId);
    }
}
