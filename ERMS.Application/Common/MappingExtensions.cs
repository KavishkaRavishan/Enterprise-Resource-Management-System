using ERMS.Domain.Entities;
using ERMS.Domain.Enums;
using ERMS.Application.DTOs.Users;
using ERMS.Application.DTOs.Projects;
using ERMS.Application.DTOs.Tasks;
using ERMS.Application.DTOs.Comments;
using ERMS.Application.DTOs.Dashboard;
using ERMS.Application.DTOs.Auth;
using ERMS.Application.DTOs.TimeLogs;
using ERMS.Application.DTOs.Attachments;

namespace ERMS.Application.Common
{
    public static class MappingExtensions
    {
        // User mappings
        public static UserDto ToDto(this User user)
        {
            return new UserDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Role = user.Role.ToString(),
                IsActive = user.IsActive,
                AvatarPath = user.AvatarPath,
                Created = user.Created,
                Updated = user.Updated
            };
        }

        public static UserTokenDto ToTokenDto(this User user)
        {
            return new UserTokenDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Role = user.Role.ToString(),
                AvatarPath = user.AvatarPath
            };
        }

        // Project mappings
        public static ProjectDto ToDto(this Project project)
        {
            var completedTasks = project.Tasks?.Count(t => t.Status == TaskItemStatus.Done) ?? 0;
            return new ProjectDto
            {
                Id = project.Id,
                Name = project.Name,
                Description = project.Description,
                StartDate = project.StartDate,
                EndDate = project.EndDate,
                Status = project.Status.ToString(),
                CreatedById = project.CreatedById,
                CreatedByName = project.CreatedBy != null
                    ? $"{project.CreatedBy.FirstName} {project.CreatedBy.LastName}"
                    : string.Empty,
                MemberCount = project.Members?.Count ?? 0,
                TaskCount = project.Tasks?.Count ?? 0,
                CompletedTaskCount = completedTasks,
                Created = project.Created,
                Updated = project.Updated,
                Members = project.Members?.Select(m => m.ToDto()).ToList() ?? new()
            };
        }

        public static ProjectMemberDto ToDto(this ProjectMember member)
        {
            return new ProjectMemberDto
            {
                UserId = member.UserId,
                FirstName = member.User?.FirstName ?? string.Empty,
                LastName = member.User?.LastName ?? string.Empty,
                Email = member.User?.Email ?? string.Empty,
                AvatarPath = member.User?.AvatarPath,
                JoinedAt = member.JoinedAt
            };
        }

        // Task mappings
        public static TaskDto ToDto(this ProjectTask task)
        {
            return new TaskDto
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                Status = task.Status.ToString(),
                Priority = task.Priority.ToString(),
                DueDate = task.DueDate,
                ProjectId = task.ProjectId,
                ProjectName = task.Project?.Name ?? string.Empty,
                AssignedToId = task.AssignedToId,
                AssignedToName = task.AssignedTo != null
                    ? $"{task.AssignedTo.FirstName} {task.AssignedTo.LastName}"
                    : null,
                AssignedToAvatar = task.AssignedTo?.AvatarPath,
                CommentCount = task.Comments?.Count ?? 0,
                Created = task.Created,
                Updated = task.Updated
            };
        }

        // Comment mappings
        public static CommentDto ToDto(this TaskComment comment)
        {
            return new CommentDto
            {
                Id = comment.Id,
                Content = comment.Content,
                AuthorId = comment.AuthorId,
                AuthorName = comment.Author != null
                    ? $"{comment.Author.FirstName} {comment.Author.LastName}"
                    : string.Empty,
                AuthorAvatar = comment.Author?.AvatarPath,
                Created = comment.Created
            };
        }

        // Notification mappings
        public static ERMS.Application.DTOs.Notifications.NotificationDto ToDto(this Notification notification)
        {
            return new ERMS.Application.DTOs.Notifications.NotificationDto
            {
                Id = notification.Id,
                Message = notification.Message,
                IsRead = notification.IsRead,
                Type = notification.Type.ToString(),
                ReferenceId = notification.ReferenceId,
                Created = notification.Created
            };
        }

        // TimeLog mappings
        public static TimeLogDto ToDto(this TimeLog timeLog)
        {
            return new TimeLogDto
            {
                Id = timeLog.Id,
                TaskId = timeLog.TaskId,
                TaskTitle = timeLog.Task?.Title ?? string.Empty,
                UserId = timeLog.UserId,
                UserName = timeLog.User != null ? $"{timeLog.User.FirstName} {timeLog.User.LastName}" : string.Empty,
                UserAvatar = timeLog.User?.AvatarPath ?? string.Empty,
                HoursSpent = timeLog.HoursSpent,
                Description = timeLog.Description,
                DateLogged = timeLog.DateLogged,
                Created = timeLog.Created
            };
        }

        // Attachment mappings
        public static AttachmentDto ToDto(this Attachment attachment)
        {
            return new AttachmentDto
            {
                Id = attachment.Id,
                FileName = attachment.FileName,
                Size = attachment.Size,
                FilePath = attachment.FilePath,
                ContentType = attachment.ContentType,
                TaskId = attachment.TaskId,
                UploadedById = attachment.UploadedById,
                UploadedByName = attachment.UploadedBy != null 
                    ? $"{attachment.UploadedBy.FirstName} {attachment.UploadedBy.LastName}" 
                    : string.Empty,
                Created = attachment.Created
            };
        }
    }
}
