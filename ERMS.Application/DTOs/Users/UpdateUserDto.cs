namespace ERMS.Application.DTOs.Users
{
    public class UpdateUserDto
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? Role { get; set; }
        public bool? IsActive { get; set; }
    }
}
