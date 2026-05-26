using ERMS.Application.Common;
using ERMS.Application.DTOs.TimeLogs;

namespace ERMS.Application.Interfaces
{
    public interface ITimeLogService
    {
        Task<ServiceResult<TimeLogDto>> LogTimeAsync(LogTimeDto dto, Guid userId);
        Task<ServiceResult<List<TimeLogDto>>> GetTimeLogsByTaskAsync(Guid taskId);
        Task<ServiceResult<List<TimeLogDto>>> GetTimeLogsByUserAsync(Guid userId);
        Task<ServiceResult<List<TimeLogDto>>> GetTimeLogsByProjectAsync(Guid projectId);
        Task<ServiceResult<bool>> DeleteTimeLogAsync(Guid id, Guid userId, string userRole);
        Task<ServiceResult<decimal>> GetTotalHoursByProjectAsync(Guid projectId);
    }
}
