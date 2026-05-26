using ERMS.Application.Common;
using ERMS.Application.DTOs.Projects;
using ERMS.Application.Interfaces;
using ERMS.Application.Interfaces.Repositories;
using ERMS.Domain.Entities;
using ERMS.Domain.Enums;

namespace ERMS.Application.Services
{
    public class ProjectService : IProjectService
    {
        private readonly IProjectRepository _projectRepository;
        private readonly INotificationService _notificationService;

        public ProjectService(IProjectRepository projectRepository, INotificationService notificationService)
        {
            _projectRepository = projectRepository;
            _notificationService = notificationService;
        }

        public async Task<ServiceResult<List<ProjectDto>>> GetAllProjectsAsync()
        {
            var projects = await _projectRepository.GetAllAsync();
            return ServiceResult<List<ProjectDto>>.Ok(projects.Select(p => p.ToDto()).ToList());
        }

        public async Task<ServiceResult<List<ProjectDto>>> GetUserProjectsAsync(Guid userId)
        {
            var projects = await _projectRepository.GetByUserIdAsync(userId);
            return ServiceResult<List<ProjectDto>>.Ok(projects.Select(p => p.ToDto()).ToList());
        }

        public async Task<ServiceResult<ProjectDto>> GetProjectByIdAsync(Guid id)
        {
            var project = await _projectRepository.GetByIdWithDetailsAsync(id);
            if (project == null)
                return ServiceResult<ProjectDto>.NotFound("Project not found");

            return ServiceResult<ProjectDto>.Ok(project.ToDto());
        }

        public async Task<ServiceResult<ProjectDto>> CreateProjectAsync(CreateProjectDto dto, Guid createdById)
        {
            var project = new Project(dto.Name, dto.Description, dto.StartDate, dto.EndDate, createdById);
            await _projectRepository.AddAsync(project);

            // Add members
            foreach (var memberId in dto.MemberIds)
            {
                var member = new ProjectMember(project.Id, memberId);
                await _projectRepository.AddMemberAsync(member);
                await _notificationService.CreateNotificationAsync(memberId, $"You have been added to the project: {project.Name}", NotificationType.ProjectAdded, project.Id);
            }

            // Reload with details
            var created = await _projectRepository.GetByIdWithDetailsAsync(project.Id);
            return ServiceResult<ProjectDto>.Created(created!.ToDto());
        }

        public async Task<ServiceResult<ProjectDto>> UpdateProjectAsync(Guid id, UpdateProjectDto dto)
        {
            var project = await _projectRepository.GetByIdWithDetailsAsync(id);
            if (project == null)
                return ServiceResult<ProjectDto>.NotFound("Project not found");

            var oldMemberIds = project.Members?.Select(m => m.UserId).ToList() ?? new List<Guid>();

            project.UpdateDetails(dto.Name, dto.Description, dto.StartDate, dto.EndDate);

            if (!string.IsNullOrEmpty(dto.Status) && Enum.TryParse<ProjectStatus>(dto.Status, true, out var status))
            {
                project.UpdateStatus(status);
            }

            await _projectRepository.UpdateAsync(project);

            // Update members if provided
            if (dto.MemberIds != null)
            {
                await _projectRepository.RemoveAllMembersAsync(id);
                foreach (var memberId in dto.MemberIds)
                {
                    var member = new ProjectMember(id, memberId);
                    await _projectRepository.AddMemberAsync(member);

                    if (!oldMemberIds.Contains(memberId))
                    {
                        await _notificationService.CreateNotificationAsync(memberId, $"You have been added to the project: {project.Name}", NotificationType.ProjectAdded, project.Id);
                    }
                }
            }

            var updated = await _projectRepository.GetByIdWithDetailsAsync(id);
            return ServiceResult<ProjectDto>.Ok(updated!.ToDto());
        }

        public async Task<ServiceResult<bool>> DeleteProjectAsync(Guid id)
        {
            var project = await _projectRepository.GetByIdAsync(id);
            if (project == null)
                return ServiceResult<bool>.NotFound("Project not found");

            await _projectRepository.DeleteAsync(id);
            return ServiceResult<bool>.Ok(true, "Project deleted successfully");
        }
    }
}
