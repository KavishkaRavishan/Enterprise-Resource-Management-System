using ERMS.Application.Common;
using ERMS.Application.DTOs.Tasks;
using ERMS.Application.Interfaces;
using ERMS.Application.Interfaces.Repositories;
using ERMS.Domain.Entities;
using ERMS.Domain.Enums;
using MediatR;

namespace ERMS.Application.Tasks.Commands
{
    public class UpdateTaskStatusCommand : IRequest<ServiceResult<TaskDto>>
    {
        public Guid Id { get; set; }
        public string Status { get; set; } = string.Empty;
    }

    public class UpdateTaskStatusCommandHandler : IRequestHandler<UpdateTaskStatusCommand, ServiceResult<TaskDto>>
    {
        private readonly ITaskRepository _taskRepository;
        private readonly INotificationService _notificationService;

        public UpdateTaskStatusCommandHandler(ITaskRepository taskRepository, INotificationService notificationService)
        {
            _taskRepository = taskRepository;
            _notificationService = notificationService;
        }

        public async Task<ServiceResult<TaskDto>> Handle(UpdateTaskStatusCommand request, CancellationToken cancellationToken)
        {
            var task = await _taskRepository.GetByIdWithDetailsAsync(request.Id);
            if (task == null)
                return ServiceResult<TaskDto>.NotFound("Task not found");

            if (!Enum.TryParse<TaskItemStatus>(request.Status, true, out var status))
                return ServiceResult<TaskDto>.Fail("Invalid status. Valid values: ToDo, InProgress, Done");

            var oldStatus = task.Status;
            task.UpdateStatus(status);
            await _taskRepository.UpdateAsync(task);

            if (task.AssignedToId.HasValue && status != oldStatus)
            {
                await _notificationService.CreateNotificationAsync(
                    task.AssignedToId.Value,
                    $"The status of task '{task.Title}' was updated to {status}",
                    NotificationType.TaskStatusChanged,
                    task.Id
                );
            }

            var updated = await _taskRepository.GetByIdWithDetailsAsync(request.Id);
            return ServiceResult<TaskDto>.Ok(updated!.ToDto());
        }
    }
}
