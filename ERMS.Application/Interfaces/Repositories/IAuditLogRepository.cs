using ERMS.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ERMS.Application.Interfaces.Repositories
{
    public interface IAuditLogRepository
    {
        Task<List<AuditLog>> GetAllWithUsersAsync();
        Task AddAsync(AuditLog log);
    }
}
