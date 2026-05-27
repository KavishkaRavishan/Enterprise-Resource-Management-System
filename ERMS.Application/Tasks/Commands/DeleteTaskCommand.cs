using ERMS.Application.Common;
using ERMS.Application.Interfaces.Repositories;
using MediatR;

namespace ERMS.Application.Tasks.Commands
{
    public class DeleteTaskCommand : IRequest<ServiceResult<bool>>
    {
        public Guid Id { get; set; }
    }

    public class DeleteTaskCommandHandler : IRequestHandler<DeleteTaskCommand, ServiceResult<bool>>
    {
        private readonly ITaskRepository _taskRepository;

        public DeleteTaskCommandHandler(ITaskRepository taskRepository)
        {
            _taskRepository = taskRepository;
        }

        public async Task<ServiceResult<bool>> Handle(DeleteTaskCommand request, CancellationToken cancellationToken)
        {
            var task = await _taskRepository.GetByIdAsync(request.Id);
            if (task == null)
                return ServiceResult<bool>.NotFound("Task not found");

            await _taskRepository.DeleteAsync(request.Id);
            return ServiceResult<bool>.Ok(true, "Task deleted successfully");
        }
    }
}
