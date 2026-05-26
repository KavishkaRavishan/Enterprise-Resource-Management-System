using ERMS.Domain.Entities;

namespace ERMS.Application.Interfaces.Repositories
{
    public interface IAttachmentRepository
    {
        Task<Attachment?> GetByIdAsync(Guid id);
        Task<List<Attachment>> GetByTaskIdAsync(Guid taskId);
        Task AddAsync(Attachment attachment);
        Task DeleteAsync(Guid id);
    }
}
