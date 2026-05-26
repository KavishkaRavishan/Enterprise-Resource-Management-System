using ERMS.Application.Common;
using ERMS.Application.DTOs.Users;
using ERMS.Application.Interfaces;
using ERMS.Application.Interfaces.Repositories;
using ERMS.Domain.Entities;

namespace ERMS.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;

        public UserService(IUserRepository userRepository, IPasswordHasher passwordHasher)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
        }

        public async Task<ServiceResult<List<UserDto>>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllAsync();
            return ServiceResult<List<UserDto>>.Ok(users.Select(u => u.ToDto()).ToList());
        }

        public async Task<ServiceResult<UserDto>> GetUserByIdAsync(Guid id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                return ServiceResult<UserDto>.NotFound("User not found");

            return ServiceResult<UserDto>.Ok(user.ToDto());
        }

        public async Task<ServiceResult<UserDto>> CreateUserAsync(CreateUserDto dto)
        {
            var existing = await _userRepository.GetByEmailAsync(dto.Email);
            if (existing != null)
                return ServiceResult<UserDto>.Fail("A user with this email already exists");

            if (!Enum.TryParse<Role>(dto.Role, true, out var role))
                return ServiceResult<UserDto>.Fail("Invalid role specified");

            var user = new User(dto.FirstName, dto.LastName, dto.Email, role);
            user.SetPassword(_passwordHasher.Hash(dto.Password));

            await _userRepository.AddAsync(user);

            return ServiceResult<UserDto>.Created(user.ToDto());
        }

        public async Task<ServiceResult<UserDto>> UpdateUserAsync(Guid id, UpdateUserDto dto)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                return ServiceResult<UserDto>.NotFound("User not found");

            user.UpdateProfile(dto.FirstName, dto.LastName);

            if (!string.IsNullOrEmpty(dto.Role) && Enum.TryParse<Role>(dto.Role, true, out var role))
            {
                user.UpdateRole(role);
            }

            if (dto.IsActive.HasValue)
            {
                if (dto.IsActive.Value) user.Activate();
                else user.Deactivate();
            }

            await _userRepository.UpdateAsync(user);

            return ServiceResult<UserDto>.Ok(user.ToDto());
        }

        public async Task<ServiceResult<bool>> DeleteUserAsync(Guid id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                return ServiceResult<bool>.NotFound("User not found");

            await _userRepository.DeleteAsync(id);

            return ServiceResult<bool>.Ok(true, "User deleted successfully");
        }

        public async Task<ServiceResult<UserDto>> UploadAvatarAsync(Guid userId, string fileName, Stream fileStream)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                return ServiceResult<UserDto>.NotFound("User not found");

            // Create uploads directory
            var uploadsDir = Path.Combine("wwwroot", "uploads", "avatars");
            Directory.CreateDirectory(uploadsDir);

            // Generate unique filename
            var extension = Path.GetExtension(fileName);
            var uniqueName = $"{userId}_{Guid.NewGuid():N}{extension}";
            var filePath = Path.Combine(uploadsDir, uniqueName);

            // Delete old avatar if exists
            if (!string.IsNullOrEmpty(user.AvatarPath))
            {
                var oldPath = Path.Combine("wwwroot", user.AvatarPath.TrimStart('/'));
                if (File.Exists(oldPath))
                    File.Delete(oldPath);
            }

            // Save new file
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await fileStream.CopyToAsync(stream);
            }

            var avatarUrl = $"/uploads/avatars/{uniqueName}";
            user.SetAvatar(avatarUrl);
            await _userRepository.UpdateAsync(user);

            return ServiceResult<UserDto>.Ok(user.ToDto());
        }
    }
}
