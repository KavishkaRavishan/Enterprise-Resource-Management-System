using ERMS.Domain.Entities;

namespace ERMS.Application.Interfaces.Repositories
{
    public interface ICommentRepository
    {
        Task<TaskComment?> GetByIdAsync(Guid id);
        Task<List<TaskComment>> GetByTaskIdAsync(Guid taskId);
        Task AddAsync(TaskComment comment);
        Task DeleteAsync(Guid id);
    }
}
