using ERMS.Application.Common;
using ERMS.Application.DTOs.Notifications;
using ERMS.Application.Interfaces;
using ERMS.Application.Interfaces.Repositories;
using ERMS.Domain.Entities;
using ERMS.Domain.Enums;

namespace ERMS.Application.Services
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notificationRepository;

        public NotificationService(INotificationRepository notificationRepository)
        {
            _notificationRepository = notificationRepository;
        }

        public async Task<ServiceResult<List<NotificationDto>>> GetUserNotificationsAsync(Guid userId)
        {
            var notifications = await _notificationRepository.GetByUserIdAsync(userId);
            var dtos = notifications
                .OrderByDescending(n => n.Created)
                .Select(n => n.ToDto())
                .ToList();

            return ServiceResult<List<NotificationDto>>.Ok(dtos);
        }

        public async Task<ServiceResult<bool>> MarkAsReadAsync(Guid id, Guid userId)
        {
            var notification = await _notificationRepository.GetByIdAsync(id);
            if (notification == null)
                return ServiceResult<bool>.NotFound("Notification not found");

            if (notification.RecipientId != userId)
                return ServiceResult<bool>.Fail("Unauthorized connection");

            notification.MarkAsRead();
            await _notificationRepository.UpdateAsync(notification);

            return ServiceResult<bool>.Ok(true, "Notification marked as read");
        }

        public async Task<ServiceResult<bool>> MarkAllAsReadAsync(Guid userId)
        {
            var notifications = await _notificationRepository.GetByUserIdAsync(userId);
            var unread = notifications.Where(n => !n.IsRead).ToList();

            if (unread.Any())
            {
                foreach (var n in unread)
                {
                    n.MarkAsRead();
                }
                await _notificationRepository.UpdateRangeAsync(unread);
            }

            return ServiceResult<bool>.Ok(true, "All notifications marked as read");
        }

        public async Task CreateNotificationAsync(Guid recipientId, string message, NotificationType type, Guid? referenceId = null)
        {
            var notification = new Notification(recipientId, message, type, referenceId);
            await _notificationRepository.AddAsync(notification);
        }
    }
}
