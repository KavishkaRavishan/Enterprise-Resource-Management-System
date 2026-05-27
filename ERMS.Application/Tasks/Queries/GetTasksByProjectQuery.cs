using ERMS.Application.Common;
using ERMS.Application.DTOs.Tasks;
using ERMS.Application.Interfaces.Repositories;
using MediatR;

namespace ERMS.Application.Tasks.Queries
{
    public class GetTasksByProjectQuery : IRequest<ServiceResult<List<TaskDto>>>
    {
        public Guid ProjectId { get; set; }
    }

    public class GetTasksByProjectQueryHandler : IRequestHandler<GetTasksByProjectQuery, ServiceResult<List<TaskDto>>>
    {
        private readonly ITaskRepository _taskRepository;

        public GetTasksByProjectQueryHandler(ITaskRepository taskRepository)
        {
            _taskRepository = taskRepository;
        }

        public async Task<ServiceResult<List<TaskDto>>> Handle(GetTasksByProjectQuery request, CancellationToken cancellationToken)
        {
            var tasks = await _taskRepository.GetByProjectIdAsync(request.ProjectId);
            return ServiceResult<List<TaskDto>>.Ok(tasks.Select(t => t.ToDto()).ToList());
        }
    }
}
