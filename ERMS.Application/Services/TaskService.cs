using ERMS.Application.Common;
using ERMS.Application.DTOs.Tasks;
using ERMS.Application.Interfaces;
using ERMS.Application.Interfaces.Repositories;
using ERMS.Domain.Entities;
using ERMS.Domain.Enums;

namespace ERMS.Application.Services
{
    public class TaskService : ITaskService
    {
        private readonly ITaskRepository _taskRepository;
        private readonly INotificationService _notificationService;

        public TaskService(ITaskRepository taskRepository, INotificationService notificationService)
        {
            _taskRepository = taskRepository;
            _notificationService = notificationService;
        }

        public async Task<ServiceResult<List<TaskDto>>> GetTasksByProjectAsync(Guid projectId)
        {
            var tasks = await _taskRepository.GetByProjectIdAsync(projectId);
            return ServiceResult<List<TaskDto>>.Ok(tasks.Select(t => t.ToDto()).ToList());
        }

        public async Task<ServiceResult<List<TaskDto>>> GetTasksByUserAsync(Guid userId)
        {
            var tasks = await _taskRepository.GetByAssignedUserIdAsync(userId);
            return ServiceResult<List<TaskDto>>.Ok(tasks.Select(t => t.ToDto()).ToList());
        }

        public async Task<ServiceResult<TaskDto>> GetTaskByIdAsync(Guid id)
        {
            var task = await _taskRepository.GetByIdWithDetailsAsync(id);
            if (task == null)
                return ServiceResult<TaskDto>.NotFound("Task not found");

            return ServiceResult<TaskDto>.Ok(task.ToDto());
        }

        public async Task<ServiceResult<TaskDto>> CreateTaskAsync(CreateTaskDto dto)
        {
            if (!Enum.TryParse<TaskPriority>(dto.Priority, true, out var priority))
                return ServiceResult<TaskDto>.Fail("Invalid priority");

            var task = new ProjectTask(dto.Title, dto.Description, dto.ProjectId, priority, dto.DueDate);

            if (dto.AssignedToId.HasValue)
            {
                task.Assign(dto.AssignedToId.Value);
            }

            await _taskRepository.AddAsync(task);

            if (dto.AssignedToId.HasValue)
            {
                await _notificationService.CreateNotificationAsync(dto.AssignedToId.Value, $"You have been assigned the task: {task.Title}", NotificationType.TaskAssigned, task.Id);
            }

            var created = await _taskRepository.GetByIdWithDetailsAsync(task.Id);
            return ServiceResult<TaskDto>.Created(created!.ToDto());
        }

        public async Task<ServiceResult<TaskDto>> UpdateTaskAsync(Guid id, UpdateTaskDto dto)
        {
            var task = await _taskRepository.GetByIdWithDetailsAsync(id);
            if (task == null)
                return ServiceResult<TaskDto>.NotFound("Task not found");

            if (!Enum.TryParse<TaskPriority>(dto.Priority, true, out var priority))
                return ServiceResult<TaskDto>.Fail("Invalid priority");

            var oldAssigneeId = task.AssignedToId;

            task.UpdateDetails(dto.Title, dto.Description, priority, dto.DueDate);
            task.Assign(dto.AssignedToId);

            await _taskRepository.UpdateAsync(task);

            if (dto.AssignedToId.HasValue && dto.AssignedToId != oldAssigneeId)
            {
                await _notificationService.CreateNotificationAsync(dto.AssignedToId.Value, $"You have been assigned the task: {task.Title}", NotificationType.TaskAssigned, task.Id);
            }

            var updated = await _taskRepository.GetByIdWithDetailsAsync(id);
            return ServiceResult<TaskDto>.Ok(updated!.ToDto());
        }

        public async Task<ServiceResult<TaskDto>> UpdateTaskStatusAsync(Guid id, UpdateTaskStatusDto dto)
        {
            var task = await _taskRepository.GetByIdWithDetailsAsync(id);
            if (task == null)
                return ServiceResult<TaskDto>.NotFound("Task not found");

            if (!Enum.TryParse<TaskItemStatus>(dto.Status, true, out var status))
                return ServiceResult<TaskDto>.Fail("Invalid status. Valid values: ToDo, InProgress, Done");

            var oldStatus = task.Status;
            task.UpdateStatus(status);
            await _taskRepository.UpdateAsync(task);

            if (task.AssignedToId.HasValue && status != oldStatus)
            {
                await _notificationService.CreateNotificationAsync(task.AssignedToId.Value, $"The status of task '{task.Title}' was updated to {status}", NotificationType.TaskStatusChanged, task.Id);
            }

            var updated = await _taskRepository.GetByIdWithDetailsAsync(id);
            return ServiceResult<TaskDto>.Ok(updated!.ToDto());
        }

        public async Task<ServiceResult<bool>> DeleteTaskAsync(Guid id)
        {
            var task = await _taskRepository.GetByIdAsync(id);
            if (task == null)
                return ServiceResult<bool>.NotFound("Task not found");

            await _taskRepository.DeleteAsync(id);
            return ServiceResult<bool>.Ok(true, "Task deleted successfully");
        }
    }
}
