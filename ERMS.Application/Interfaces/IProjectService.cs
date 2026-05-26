using ERMS.Application.Common;
using ERMS.Application.DTOs.Projects;

namespace ERMS.Application.Interfaces
{
    public interface IProjectService
    {
        Task<ServiceResult<List<ProjectDto>>> GetAllProjectsAsync();
        Task<ServiceResult<List<ProjectDto>>> GetUserProjectsAsync(Guid userId);
        Task<ServiceResult<ProjectDto>> GetProjectByIdAsync(Guid id);
        Task<ServiceResult<ProjectDto>> CreateProjectAsync(CreateProjectDto dto, Guid createdById);
        Task<ServiceResult<ProjectDto>> UpdateProjectAsync(Guid id, UpdateProjectDto dto);
        Task<ServiceResult<bool>> DeleteProjectAsync(Guid id);
    }
}
