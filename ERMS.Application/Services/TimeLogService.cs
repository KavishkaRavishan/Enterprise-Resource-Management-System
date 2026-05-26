using ERMS.Application.Common;
using ERMS.Application.DTOs.TimeLogs;
using ERMS.Application.Interfaces;
using ERMS.Application.Interfaces.Repositories;
using ERMS.Domain.Entities;

namespace ERMS.Application.Services
{
    public class TimeLogService : ITimeLogService
    {
        private readonly ITimeLogRepository _timeLogRepository;
        private readonly ITaskRepository _taskRepository;

        public TimeLogService(ITimeLogRepository timeLogRepository, ITaskRepository taskRepository)
        {
            _timeLogRepository = timeLogRepository;
            _taskRepository = taskRepository;
        }

        public async Task<ServiceResult<TimeLogDto>> LogTimeAsync(LogTimeDto dto, Guid userId)
        {
            var task = await _taskRepository.GetByIdAsync(dto.TaskId);
            if (task == null)
                return ServiceResult<TimeLogDto>.NotFound("Task not found");

            var timeLog = new TimeLog(dto.TaskId, userId, dto.HoursSpent, dto.Description, dto.DateLogged);
            await _timeLogRepository.AddAsync(timeLog);

            var created = await _timeLogRepository.GetByIdAsync(timeLog.Id);
            return ServiceResult<TimeLogDto>.Created(created!.ToDto());
        }

        public async Task<ServiceResult<List<TimeLogDto>>> GetTimeLogsByTaskAsync(Guid taskId)
        {
            var logs = await _timeLogRepository.GetByTaskIdAsync(taskId);
            return ServiceResult<List<TimeLogDto>>.Ok(logs.Select(l => l.ToDto()).ToList());
        }

        public async Task<ServiceResult<List<TimeLogDto>>> GetTimeLogsByUserAsync(Guid userId)
        {
            var logs = await _timeLogRepository.GetByUserIdAsync(userId);
            return ServiceResult<List<TimeLogDto>>.Ok(logs.Select(l => l.ToDto()).ToList());
        }

        public async Task<ServiceResult<List<TimeLogDto>>> GetTimeLogsByProjectAsync(Guid projectId)
        {
            var logs = await _timeLogRepository.GetByProjectIdAsync(projectId);
            return ServiceResult<List<TimeLogDto>>.Ok(logs.Select(l => l.ToDto()).ToList());
        }

        public async Task<ServiceResult<bool>> DeleteTimeLogAsync(Guid id, Guid userId, string userRole)
        {
            var log = await _timeLogRepository.GetByIdAsync(id);
            if (log == null)
                return ServiceResult<bool>.NotFound("Time log not found");

            // Authorization: Admins/Managers can delete anything. Employees can only delete their own.
            if (userRole != "Admin" && userRole != "Manager" && log.UserId != userId)
                return ServiceResult<bool>.Fail("Unauthorized: You can only delete your own time logs");

            await _timeLogRepository.DeleteAsync(id);
            return ServiceResult<bool>.Ok(true, "Time log deleted successfully");
        }

        public async Task<ServiceResult<decimal>> GetTotalHoursByProjectAsync(Guid projectId)
        {
            var total = await _timeLogRepository.GetTotalHoursByProjectAsync(projectId);
            return ServiceResult<decimal>.Ok(total);
        }
    }
}
