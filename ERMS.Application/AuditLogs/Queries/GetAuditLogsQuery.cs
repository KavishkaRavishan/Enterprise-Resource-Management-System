using ERMS.Application.Common;
using ERMS.Application.DTOs.AuditLogs;
using ERMS.Application.Interfaces.Repositories;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ERMS.Application.AuditLogs.Queries
{
    public class GetAuditLogsQuery : IRequest<ServiceResult<List<AuditLogDto>>>
    {
    }

    public class GetAuditLogsQueryHandler : IRequestHandler<GetAuditLogsQuery, ServiceResult<List<AuditLogDto>>>
    {
        private readonly IAuditLogRepository _auditLogRepository;

        public GetAuditLogsQueryHandler(IAuditLogRepository auditLogRepository)
        {
            _auditLogRepository = auditLogRepository;
        }

        public async Task<ServiceResult<List<AuditLogDto>>> Handle(GetAuditLogsQuery request, CancellationToken cancellationToken)
        {
            var logs = await _auditLogRepository.GetAllWithUsersAsync();
            var dtos = logs.Select(l => l.ToDto()).ToList();
            return ServiceResult<List<AuditLogDto>>.Ok(dtos);
        }
    }
}
