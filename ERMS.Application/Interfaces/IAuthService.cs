using ERMS.Application.Common;
using ERMS.Application.DTOs.Auth;

namespace ERMS.Application.Interfaces
{
    public interface IAuthService
    {
        Task<ServiceResult<AuthResponseDto>> LoginAsync(LoginDto dto);
        Task<ServiceResult<AuthResponseDto>> RefreshTokenAsync(RefreshTokenDto dto);
        Task<ServiceResult<bool>> LogoutAsync(Guid userId);
    }
}
