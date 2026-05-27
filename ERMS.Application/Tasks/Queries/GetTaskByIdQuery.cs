using ERMS.Application.Common;
using ERMS.Application.DTOs.Tasks;
using ERMS.Application.Interfaces.Repositories;
using MediatR;

namespace ERMS.Application.Tasks.Queries
{
    public class GetTaskByIdQuery : IRequest<ServiceResult<TaskDto>>
    {
        public Guid Id { get; set; }
    }

    public class GetTaskByIdQueryHandler : IRequestHandler<GetTaskByIdQuery, ServiceResult<TaskDto>>
    {
        private readonly ITaskRepository _taskRepository;

        public GetTaskByIdQueryHandler(ITaskRepository taskRepository)
        {
            _taskRepository = taskRepository;
        }

        public async Task<ServiceResult<TaskDto>> Handle(GetTaskByIdQuery request, CancellationToken cancellationToken)
        {
            var task = await _taskRepository.GetByIdWithDetailsAsync(request.Id);
            if (task == null)
                return ServiceResult<TaskDto>.NotFound("Task not found");

            return ServiceResult<TaskDto>.Ok(task.ToDto());
        }
    }
}
