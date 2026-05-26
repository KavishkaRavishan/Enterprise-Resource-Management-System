using ERMS.Application.Common;
using ERMS.Application.DTOs.Tasks;

namespace ERMS.Application.Interfaces
{
    public interface ITaskService
    {
        Task<ServiceResult<List<TaskDto>>> GetTasksByProjectAsync(Guid projectId);
        Task<ServiceResult<List<TaskDto>>> GetTasksByUserAsync(Guid userId);
        Task<ServiceResult<TaskDto>> GetTaskByIdAsync(Guid id);
        Task<ServiceResult<TaskDto>> CreateTaskAsync(CreateTaskDto dto);
        Task<ServiceResult<TaskDto>> UpdateTaskAsync(Guid id, UpdateTaskDto dto);
        Task<ServiceResult<TaskDto>> UpdateTaskStatusAsync(Guid id, UpdateTaskStatusDto dto);
        Task<ServiceResult<bool>> DeleteTaskAsync(Guid id);
    }
}
