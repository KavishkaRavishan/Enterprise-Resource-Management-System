namespace ERMS.Domain.Entities
{
    public class User : BaseEntity
    {
        public string FirstName { get; private set; } = string.Empty;
        public string LastName { get; private set; } = string.Empty;
        public string Email { get; private set; } = string.Empty;
        public string PasswordHash { get; private set; } = string.Empty;
        public Role Role { get; private set; }
        public bool IsActive { get; private set; } = true;
        public string? AvatarPath { get; private set; }
        public string? RefreshToken { get; private set; }
        public DateTime? RefreshTokenExpiry { get; private set; }

        // Navigation properties
        public ICollection<ProjectMember> ProjectMemberships { get; private set; } = new List<ProjectMember>();
        public ICollection<ProjectTask> AssignedTasks { get; private set; } = new List<ProjectTask>();
        public ICollection<TaskComment> Comments { get; private set; } = new List<TaskComment>();
        public ICollection<Notification> Notifications { get; private set; } = new List<Notification>();

        private User() { } // For EF Core

        public User(string firstName, string lastName, string email, Role role)
        {
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            Role = role;
        }

        public void SetPassword(string passwordHash)
        {
            PasswordHash = passwordHash;
            Updated = DateTime.UtcNow;
        }

        public void UpdateProfile(string firstName, string lastName)
        {
            FirstName = firstName;
            LastName = lastName;
            Updated = DateTime.UtcNow;
        }

        public void UpdateRole(Role role)
        {
            Role = role;
            Updated = DateTime.UtcNow;
        }

        public void Deactivate()
        {
            IsActive = false;
            Updated = DateTime.UtcNow;
        }

        public void Activate()
        {
            IsActive = true;
            Updated = DateTime.UtcNow;
        }

        public void SetRefreshToken(string token, DateTime expiry)
        {
            RefreshToken = token;
            RefreshTokenExpiry = expiry;
            Updated = DateTime.UtcNow;
        }

        public void ClearRefreshToken()
        {
            RefreshToken = null;
            RefreshTokenExpiry = null;
            Updated = DateTime.UtcNow;
        }

        public void SetAvatar(string? avatarPath)
        {
            AvatarPath = avatarPath;
            Updated = DateTime.UtcNow;
        }
    }
}