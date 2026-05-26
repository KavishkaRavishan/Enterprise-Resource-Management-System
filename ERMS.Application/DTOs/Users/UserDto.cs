namespace ERMS.Application.DTOs.Users
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public string? AvatarPath { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
    }
}
