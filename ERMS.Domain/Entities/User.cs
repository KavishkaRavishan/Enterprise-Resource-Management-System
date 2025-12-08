using ERMS.Domain.Entities;

namespace ERMS.Domain.Entities
{
    public class User
    {
        public Guid Id { get; private set; }
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string Email { get; private set; }
        public string PasswordHash { get; private set; }
        public Role Role { get; private set; }

        private User() { } // For EF Core

        public User(string firstName, string lastName, string email, Role role)
        {
            Id = Guid.NewGuid();
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            Role = role;
        }

        public void SetPassword(string passwordHash)
        {
            PasswordHash = passwordHash;
        }
    }
}
