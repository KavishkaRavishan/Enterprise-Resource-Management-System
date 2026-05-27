using ERMS.Application.Common;
using ERMS.Application.Interfaces.Repositories;
using ERMS.Application.Tasks.Queries;
using ERMS.Domain.Entities;
using ERMS.Domain.Enums;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ERMS.Tests.Queries
{
    public class GetTasksByProjectQueryTests
    {
        [Fact]
        public async Task Should_Return_Tasks_From_Repository_Mapped_To_Dtos()
        {
            // Arrange
            var projectId = Guid.NewGuid();
            
            var mockTasks = new List<ProjectTask>
            {
                new ProjectTask("Task 1", "Description 1", projectId, TaskPriority.High, null),
                new ProjectTask("Task 2", "Description 2", projectId, TaskPriority.Low, null)
            };

            var mockRepo = new Mock<ITaskRepository>();
            mockRepo.Setup(r => r.GetByProjectIdAsync(projectId))
                    .ReturnsAsync(mockTasks);

            var handler = new GetTasksByProjectQueryHandler(mockRepo.Object);
            var query = new GetTasksByProjectQuery { ProjectId = projectId };

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.Equal(200, result.StatusCode);
            Assert.NotNull(result.Data);
            Assert.Equal(2, result.Data.Count);

            // Verify mapping to DTOs
            Assert.Equal("Task 1", result.Data[0].Title);
            Assert.Equal(TaskPriority.High.ToString(), result.Data[0].Priority);
            Assert.Equal("Task 2", result.Data[1].Title);
            Assert.Equal(TaskPriority.Low.ToString(), result.Data[1].Priority);

            // Verify repo was invoked
            mockRepo.Verify(r => r.GetByProjectIdAsync(projectId), Times.Once);
        }
    }
}
