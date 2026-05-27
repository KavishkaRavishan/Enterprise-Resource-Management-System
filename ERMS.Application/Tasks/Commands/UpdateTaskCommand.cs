using ERMS.Application.Common;
using ERMS.Application.DTOs.Tasks;
using ERMS.Application.Interfaces;
using ERMS.Application.Interfaces.Repositories;
using ERMS.Domain.Entities;
using ERMS.Domain.Enums;
using MediatR;

namespace ERMS.Application.Tasks.Commands
{
    public class UpdateTaskCommand : IRequest<ServiceResult<TaskDto>>
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Priority { get; set; } = "Medium";
        public DateTime? DueDate { get; set; }
        public Guid? AssignedToId { get; set; }
    }

    public class UpdateTaskCommandHandler : IRequestHandler<UpdateTaskCommand, ServiceResult<TaskDto>>
    {
        private readonly ITaskRepository _taskRepository;
        private readonly INotificationService _notificationService;

        public UpdateTaskCommandHandler(ITaskRepository taskRepository, INotificationService notificationService)
        {
            _taskRepository = taskRepository;
            _notificationService = notificationService;
        }

        public async Task<ServiceResult<TaskDto>> Handle(UpdateTaskCommand request, CancellationToken cancellationToken)
        {
            var task = await _taskRepository.GetByIdWithDetailsAsync(request.Id);
            if (task == null)
                return ServiceResult<TaskDto>.NotFound("Task not found");

            if (!Enum.TryParse<TaskPriority>(request.Priority, true, out var priority))
                return ServiceResult<TaskDto>.Fail("Invalid priority");

            var oldAssigneeId = task.AssignedToId;

            task.UpdateDetails(request.Title, request.Description, priority, request.DueDate);
            task.Assign(request.AssignedToId);

            await _taskRepository.UpdateAsync(task);

            if (request.AssignedToId.HasValue && request.AssignedToId != oldAssigneeId)
            {
                await _notificationService.CreateNotificationAsync(
                    request.AssignedToId.Value,
                    $"You have been assigned the task: {task.Title}",
                    NotificationType.TaskAssigned,
                    task.Id
                );
            }

            var updated = await _taskRepository.GetByIdWithDetailsAsync(request.Id);
            return ServiceResult<TaskDto>.Ok(updated!.ToDto());
        }
    }
}
