using ERMS.Application.Common;
using ERMS.Application.DTOs.Notifications;
using ERMS.Domain.Enums;

namespace ERMS.Application.Interfaces
{
    public interface INotificationService
    {
        Task<ServiceResult<List<NotificationDto>>> GetUserNotificationsAsync(Guid userId);
        Task<ServiceResult<bool>> MarkAsReadAsync(Guid id, Guid userId);
        Task<ServiceResult<bool>> MarkAllAsReadAsync(Guid userId);
        Task CreateNotificationAsync(Guid recipientId, string message, NotificationType type, Guid? referenceId = null);
    }
}
