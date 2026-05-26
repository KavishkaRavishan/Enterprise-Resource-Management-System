using System.Security.Claims;
using ERMS.Application.DTOs.TimeLogs;
using ERMS.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ERMS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TimeLogsController : ControllerBase
    {
        private readonly ITimeLogService _timeLogService;

        public TimeLogsController(ITimeLogService timeLogService)
        {
            _timeLogService = timeLogService;
        }

        [HttpPost]
        public async Task<IActionResult> LogTime([FromBody] LogTimeDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = GetCurrentUserId();
            var result = await _timeLogService.LogTimeAsync(dto, userId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("task/{taskId:guid}")]
        public async Task<IActionResult> GetByTask(Guid taskId)
        {
            var result = await _timeLogService.GetTimeLogsByTaskAsync(taskId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("project/{projectId:guid}")]
        public async Task<IActionResult> GetByProject(Guid projectId)
        {
            var result = await _timeLogService.GetTimeLogsByProjectAsync(projectId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("user/{userId:guid}")]
        public async Task<IActionResult> GetByUser(Guid userId)
        {
            var result = await _timeLogService.GetTimeLogsByUserAsync(userId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var userId = GetCurrentUserId();
            var userRole = GetCurrentUserRole();
            var result = await _timeLogService.DeleteTimeLogAsync(id, userId, userRole);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("project/{projectId:guid}/total")]
        public async Task<IActionResult> GetTotalHours(Guid projectId)
        {
            var result = await _timeLogService.GetTotalHoursByProjectAsync(projectId);
            return StatusCode(result.StatusCode, result);
        }

        private Guid GetCurrentUserId()
        {
            var nameIdentifier = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(nameIdentifier, out var id) ? id : Guid.Empty;
        }

        private string GetCurrentUserRole()
        {
            return User.FindFirstValue(ClaimTypes.Role) ?? string.Empty;
        }
    }
}
