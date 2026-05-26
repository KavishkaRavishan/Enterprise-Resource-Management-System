using System.Security.Claims;
using ERMS.Application.DTOs.Projects;
using ERMS.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ERMS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProjectsController : ControllerBase
    {
        private readonly IProjectService _projectService;

        public ProjectsController(IProjectService projectService)
        {
            _projectService = projectService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var role = User.FindFirstValue(ClaimTypes.Role);
            if (role == "Admin" || role == "Manager")
            {
                var result = await _projectService.GetAllProjectsAsync();
                return StatusCode(result.StatusCode, result);
            }
            else
            {
                var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                var result = await _projectService.GetUserProjectsAsync(userId);
                return StatusCode(result.StatusCode, result);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _projectService.GetProjectByIdAsync(id);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost]
        [Authorize(Policy = "ManagerOrAdmin")]
        public async Task<IActionResult> Create([FromBody] CreateProjectDto dto)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await _projectService.CreateProjectAsync(dto, userId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "ManagerOrAdmin")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProjectDto dto)
        {
            var result = await _projectService.UpdateProjectAsync(id, dto);
            return StatusCode(result.StatusCode, result);
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "ManagerOrAdmin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _projectService.DeleteProjectAsync(id);
            return StatusCode(result.StatusCode, result);
        }
    }
}
