namespace ERMS.Domain.Entities
{
    public class User : BaseEntity
    {
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string Email { get; private set; }
        public string PasswordHash { get; private set; }
        public Role Role { get; private set; }

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
    }
}