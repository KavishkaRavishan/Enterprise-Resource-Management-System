using System.Security.Claims;
using ERMS.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ERMS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AttachmentsController : ControllerBase
    {
        private readonly IAttachmentService _attachmentService;

        public AttachmentsController(IAttachmentService attachmentService)
        {
            _attachmentService = attachmentService;
        }

        [HttpPost("task/{taskId:guid}")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadAttachment(Guid taskId, [FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file was uploaded.");

            var userId = GetCurrentUserId();

            using (var stream = file.OpenReadStream())
            {
                var result = await _attachmentService.UploadAttachmentAsync(
                    taskId, 
                    stream, 
                    file.FileName, 
                    file.ContentType, 
                    file.Length, 
                    userId
                );
                return StatusCode(result.StatusCode, result);
            }
        }

        [HttpGet("task/{taskId:guid}")]
        public async Task<IActionResult> GetByTask(Guid taskId)
        {
            var result = await _attachmentService.GetAttachmentsByTaskAsync(taskId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var userId = GetCurrentUserId();
            var userRole = GetCurrentUserRole();
            var result = await _attachmentService.DeleteAttachmentAsync(id, userId, userRole);
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
