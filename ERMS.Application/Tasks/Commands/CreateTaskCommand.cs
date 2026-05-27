using ERMS.Application.Common;
using ERMS.Application.DTOs.Tasks;
using ERMS.Application.Interfaces;
using ERMS.Application.Interfaces.Repositories;
using ERMS.Domain.Entities;
using ERMS.Domain.Enums;
using MediatR;

namespace ERMS.Application.Tasks.Commands
{
    public class CreateTaskCommand : IRequest<ServiceResult<TaskDto>>
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Priority { get; set; } = "Medium";
        public DateTime? DueDate { get; set; }
        public Guid ProjectId { get; set; }
        public Guid? AssignedToId { get; set; }
    }

    public class CreateTaskCommandHandler : IRequestHandler<CreateTaskCommand, ServiceResult<TaskDto>>
    {
        private readonly ITaskRepository _taskRepository;
        private readonly INotificationService _notificationService;

        public CreateTaskCommandHandler(ITaskRepository taskRepository, INotificationService notificationService)
        {
            _taskRepository = taskRepository;
            _notificationService = notificationService;
        }

        public async Task<ServiceResult<TaskDto>> Handle(CreateTaskCommand request, CancellationToken cancellationToken)
        {
            if (!Enum.TryParse<TaskPriority>(request.Priority, true, out var priority))
                return ServiceResult<TaskDto>.Fail("Invalid priority");

            var task = new ProjectTask(request.Title, request.Description, request.ProjectId, priority, request.DueDate);

            if (request.AssignedToId.HasValue)
            {
                task.Assign(request.AssignedToId.Value);
            }

            await _taskRepository.AddAsync(task);

            if (request.AssignedToId.HasValue)
            {
                await _notificationService.CreateNotificationAsync(
                    request.AssignedToId.Value,
                    $"You have been assigned the task: {task.Title}",
                    NotificationType.TaskAssigned,
                    task.Id
                );
            }

            var created = await _taskRepository.GetByIdWithDetailsAsync(task.Id);
            return ServiceResult<TaskDto>.Created(created!.ToDto());
        }
    }
}
