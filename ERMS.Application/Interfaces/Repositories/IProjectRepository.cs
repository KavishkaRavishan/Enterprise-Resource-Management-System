using ERMS.Domain.Entities;

namespace ERMS.Application.Interfaces.Repositories
{
    public interface IProjectRepository
    {
        Task<Project?> GetByIdAsync(Guid id);
        Task<Project?> GetByIdWithDetailsAsync(Guid id);
        Task<List<Project>> GetAllAsync();
        Task<List<Project>> GetByUserIdAsync(Guid userId);
        Task AddAsync(Project project);
        Task UpdateAsync(Project project);
        Task DeleteAsync(Guid id);
        Task AddMemberAsync(ProjectMember member);
        Task RemoveMemberAsync(Guid projectId, Guid userId);
        Task RemoveAllMembersAsync(Guid projectId);
        Task<int> GetCountAsync();
        Task<int> GetActiveCountAsync();
    }
}
