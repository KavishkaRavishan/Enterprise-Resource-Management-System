using ERMS.Application.Common;
using ERMS.Application.DTOs.Users;

namespace ERMS.Application.Interfaces
{
    public interface IUserService
    {
        Task<ServiceResult<List<UserDto>>> GetAllUsersAsync();
        Task<ServiceResult<UserDto>> GetUserByIdAsync(Guid id);
        Task<ServiceResult<UserDto>> CreateUserAsync(CreateUserDto dto);
        Task<ServiceResult<UserDto>> UpdateUserAsync(Guid id, UpdateUserDto dto);
        Task<ServiceResult<bool>> DeleteUserAsync(Guid id);
        Task<ServiceResult<UserDto>> UploadAvatarAsync(Guid userId, string fileName, Stream fileStream);
    }
}
