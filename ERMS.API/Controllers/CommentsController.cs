using System.Security.Claims;
using ERMS.Application.DTOs.Comments;
using ERMS.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ERMS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CommentsController : ControllerBase
    {
        private readonly ICommentService _commentService;

        public CommentsController(ICommentService commentService)
        {
            _commentService = commentService;
        }

        [HttpGet("task/{taskId}")]
        public async Task<IActionResult> GetByTask(Guid taskId)
        {
            var result = await _commentService.GetCommentsByTaskAsync(taskId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("task/{taskId}")]
        public async Task<IActionResult> AddComment(Guid taskId, [FromBody] CreateCommentDto dto)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await _commentService.AddCommentAsync(taskId, dto, userId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await _commentService.DeleteCommentAsync(id, userId);
            return StatusCode(result.StatusCode, result);
        }
    }
}
