using ERMS.Application.Common;
using ERMS.Application.DTOs.Auth;
using ERMS.Application.Interfaces;
using ERMS.Application.Interfaces.Repositories;

namespace ERMS.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly IPasswordHasher _passwordHasher;

        public AuthService(
            IUserRepository userRepository,
            IJwtTokenGenerator jwtTokenGenerator,
            IPasswordHasher passwordHasher)
        {
            _userRepository = userRepository;
            _jwtTokenGenerator = jwtTokenGenerator;
            _passwordHasher = passwordHasher;
        }

        public async Task<ServiceResult<AuthResponseDto>> LoginAsync(LoginDto dto)
        {
            var user = await _userRepository.GetByEmailAsync(dto.Email);
            if (user == null)
                return ServiceResult<AuthResponseDto>.Unauthorized("Invalid email or password");

            if (!user.IsActive)
                return ServiceResult<AuthResponseDto>.Forbidden("Account is deactivated");

            if (!_passwordHasher.Verify(dto.Password, user.PasswordHash))
                return ServiceResult<AuthResponseDto>.Unauthorized("Invalid email or password");

            var accessToken = _jwtTokenGenerator.GenerateAccessToken(user);
            var refreshToken = _jwtTokenGenerator.GenerateRefreshToken();

            user.SetRefreshToken(refreshToken, DateTime.UtcNow.AddDays(7));
            await _userRepository.UpdateAsync(user);

            return ServiceResult<AuthResponseDto>.Ok(new AuthResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                User = user.ToTokenDto()
            });
        }

        public async Task<ServiceResult<AuthResponseDto>> RefreshTokenAsync(RefreshTokenDto dto)
        {
            var user = await _userRepository.GetByRefreshTokenAsync(dto.RefreshToken);
            if (user == null || user.RefreshTokenExpiry < DateTime.UtcNow)
                return ServiceResult<AuthResponseDto>.Unauthorized("Invalid or expired refresh token");

            var accessToken = _jwtTokenGenerator.GenerateAccessToken(user);
            var newRefreshToken = _jwtTokenGenerator.GenerateRefreshToken();

            user.SetRefreshToken(newRefreshToken, DateTime.UtcNow.AddDays(7));
            await _userRepository.UpdateAsync(user);

            return ServiceResult<AuthResponseDto>.Ok(new AuthResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = newRefreshToken,
                User = user.ToTokenDto()
            });
        }

        public async Task<ServiceResult<bool>> LogoutAsync(Guid userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                return ServiceResult<bool>.NotFound("User not found");

            user.ClearRefreshToken();
            await _userRepository.UpdateAsync(user);

            return ServiceResult<bool>.Ok(true, "Logged out successfully");
        }
    }
}
