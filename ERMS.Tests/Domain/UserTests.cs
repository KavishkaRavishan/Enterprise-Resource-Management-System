using ERMS.Domain.Entities;
using ERMS.Domain.Enums;
using Xunit;

namespace ERMS.Tests.Domain
{
    public class UserTests
    {
        [Fact]
        public void User_Should_Be_Active_Upon_Initialization()
        {
            // Arrange & Act
            var user = new User("John", "Doe", "john.doe@example.com", Role.Employee);

            // Assert
            Assert.True(user.IsActive);
        }

        [Fact]
        public void User_Should_Be_Deactivated_When_Deactivate_Is_Called()
        {
            // Arrange
            var user = new User("John", "Doe", "john.doe@example.com", Role.Employee);

            // Act
            user.Deactivate();

            // Assert
            Assert.False(user.IsActive);
        }

        [Fact]
        public void User_Should_Be_Activated_When_Activate_Is_Called()
        {
            // Arrange
            var user = new User("John", "Doe", "john.doe@example.com", Role.Employee);
            user.Deactivate();

            // Act
            user.Activate();

            // Assert
            Assert.True(user.IsActive);
        }

        [Fact]
        public void User_Should_Update_Profile_Details()
        {
            // Arrange
            var user = new User("John", "Doe", "john.doe@example.com", Role.Employee);

            // Act
            user.UpdateProfile("Jane", "Smith");

            // Assert
            Assert.Equal("Jane", user.FirstName);
            Assert.Equal("Smith", user.LastName);
        }

        [Fact]
        public void User_Should_Set_And_Clear_RefreshToken()
        {
            // Arrange
            var user = new User("John", "Doe", "john.doe@example.com", Role.Employee);
            var expiry = DateTime.UtcNow.AddDays(7);

            // Act
            user.SetRefreshToken("secret-token", expiry);

            // Assert
            Assert.Equal("secret-token", user.RefreshToken);
            Assert.Equal(expiry, user.RefreshTokenExpiry);

            // Act: clear token
            user.ClearRefreshToken();

            // Assert: cleared
            Assert.Null(user.RefreshToken);
            Assert.Null(user.RefreshTokenExpiry);
        }
    }
}
