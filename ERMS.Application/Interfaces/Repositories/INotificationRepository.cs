using ERMS.Domain.Entities;

namespace ERMS.Application.Interfaces.Repositories
{
    public interface INotificationRepository
    {
        Task<Notification?> GetByIdAsync(Guid id);
        Task<List<Notification>> GetByUserIdAsync(Guid userId);
        Task AddAsync(Notification notification);
        Task UpdateAsync(Notification notification);
        Task UpdateRangeAsync(List<Notification> notifications);
        Task DeleteAsync(Guid id);
    }
}
