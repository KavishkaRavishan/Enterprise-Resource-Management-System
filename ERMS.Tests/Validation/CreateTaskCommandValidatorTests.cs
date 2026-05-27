using ERMS.Application.Tasks.Commands;
using System;
using Xunit;

namespace ERMS.Tests.Validation
{
    public class CreateTaskCommandValidatorTests
    {
        private readonly CreateTaskCommandValidator _validator;

        public CreateTaskCommandValidatorTests()
        {
            _validator = new CreateTaskCommandValidator();
        }

        [Fact]
        public void Should_Fail_Validation_When_Title_Is_Empty()
        {
            // Arrange
            var command = new CreateTaskCommand 
            { 
                Title = string.Empty, 
                ProjectId = Guid.NewGuid(), 
                Priority = "Medium"
            };

            // Act
            var result = _validator.Validate(command);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "Title");
        }

        [Fact]
        public void Should_Fail_Validation_When_Title_Exceeds_200_Characters()
        {
            // Arrange
            var command = new CreateTaskCommand 
            { 
                Title = new string('t', 201), 
                ProjectId = Guid.NewGuid(), 
                Priority = "Medium"
            };

            // Act
            var result = _validator.Validate(command);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "Title");
        }

        [Fact]
        public void Should_Fail_Validation_When_ProjectId_Is_Empty()
        {
            // Arrange
            var command = new CreateTaskCommand 
            { 
                Title = "Valid Title", 
                ProjectId = Guid.Empty, 
                Priority = "Medium"
            };

            // Act
            var result = _validator.Validate(command);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "ProjectId");
        }

        [Fact]
        public void Should_Pass_Validation_When_Command_Is_Valid()
        {
            // Arrange
            var command = new CreateTaskCommand 
            { 
                Title = "Write Clean Code", 
                ProjectId = Guid.NewGuid(), 
                Priority = "High"
            };

            // Act
            var result = _validator.Validate(command);

            // Assert
            Assert.True(result.IsValid);
            Assert.Empty(result.Errors);
        }
    }
}
