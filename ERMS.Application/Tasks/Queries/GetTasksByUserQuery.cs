using ERMS.Application.Common;
using ERMS.Application.DTOs.Tasks;
using ERMS.Application.Interfaces.Repositories;
using MediatR;

namespace ERMS.Application.Tasks.Queries
{
    public class GetTasksByUserQuery : IRequest<ServiceResult<List<TaskDto>>>
    {
        public Guid UserId { get; set; }
    }

    public class GetTasksByUserQueryHandler : IRequestHandler<GetTasksByUserQuery, ServiceResult<List<TaskDto>>>
    {
        private readonly ITaskRepository _taskRepository;

        public GetTasksByUserQueryHandler(ITaskRepository taskRepository)
        {
            _taskRepository = taskRepository;
        }

        public async Task<ServiceResult<List<TaskDto>>> Handle(GetTasksByUserQuery request, CancellationToken cancellationToken)
        {
            var tasks = await _taskRepository.GetByAssignedUserIdAsync(request.UserId);
            return ServiceResult<List<TaskDto>>.Ok(tasks.Select(t => t.ToDto()).ToList());
        }
    }
}
