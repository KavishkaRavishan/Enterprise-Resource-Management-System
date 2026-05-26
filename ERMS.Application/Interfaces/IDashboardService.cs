using ERMS.Application.Common;
using ERMS.Application.DTOs.Dashboard;

namespace ERMS.Application.Interfaces
{
    public interface IDashboardService
    {
        Task<ServiceResult<DashboardDto>> GetDashboardAsync();
    }
}
