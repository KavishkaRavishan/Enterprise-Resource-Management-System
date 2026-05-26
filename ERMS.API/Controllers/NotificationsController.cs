using System.Security.Claims;
using ERMS.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ERMS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationsController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpGet]
        public async Task<IActionResult> GetUserNotifications()
        {
            var userId = GetCurrentUserId();
            var result = await _notificationService.GetUserNotificationsAsync(userId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPatch("{id:guid}/read")]
        public async Task<IActionResult> MarkAsRead(Guid id)
        {
            var userId = GetCurrentUserId();
            var result = await _notificationService.MarkAsReadAsync(id, userId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("read-all")]
        public async Task<IActionResult> MarkAllAsRead()
        {
            var userId = GetCurrentUserId();
            var result = await _notificationService.MarkAllAsReadAsync(userId);
            return StatusCode(result.StatusCode, result);
        }

        private Guid GetCurrentUserId()
        {
            var nameIdentifier = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(nameIdentifier, out var id) ? id : Guid.Empty;
        }
    }
}
