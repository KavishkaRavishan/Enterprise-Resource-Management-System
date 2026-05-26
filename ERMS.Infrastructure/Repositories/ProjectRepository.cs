using ERMS.Application.Interfaces.Repositories;
using ERMS.Domain.Entities;
using ERMS.Domain.Enums;
using ERMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ERMS.Infrastructure.Repositories
{
    public class ProjectRepository : IProjectRepository
    {
        private readonly AppDbContext _context;

        public ProjectRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Project?> GetByIdAsync(Guid id)
        {
            return await _context.Projects.FindAsync(id);
        }

        public async Task<Project?> GetByIdWithDetailsAsync(Guid id)
        {
            return await _context.Projects
                .Include(p => p.CreatedBy)
                .Include(p => p.Members)
                    .ThenInclude(m => m.User)
                .Include(p => p.Tasks)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<List<Project>> GetAllAsync()
        {
            return await _context.Projects
                .Include(p => p.CreatedBy)
                .Include(p => p.Members)
                    .ThenInclude(m => m.User)
                .Include(p => p.Tasks)
                .OrderByDescending(p => p.Created)
                .ToListAsync();
        }

        public async Task<List<Project>> GetByUserIdAsync(Guid userId)
        {
            return await _context.Projects
                .Include(p => p.CreatedBy)
                .Include(p => p.Members)
                    .ThenInclude(m => m.User)
                .Include(p => p.Tasks)
                .Where(p => p.CreatedById == userId || p.Members.Any(m => m.UserId == userId))
                .OrderByDescending(p => p.Created)
                .ToListAsync();
        }

        public async Task AddAsync(Project project)
        {
            await _context.Projects.AddAsync(project);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Project project)
        {
            _context.Projects.Update(project);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var project = await _context.Projects.FindAsync(id);
            if (project != null)
            {
                _context.Projects.Remove(project);
                await _context.SaveChangesAsync();
            }
        }

        public async Task AddMemberAsync(ProjectMember member)
        {
            await _context.ProjectMembers.AddAsync(member);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveMemberAsync(Guid projectId, Guid userId)
        {
            var member = await _context.ProjectMembers
                .FirstOrDefaultAsync(pm => pm.ProjectId == projectId && pm.UserId == userId);
            if (member != null)
            {
                _context.ProjectMembers.Remove(member);
                await _context.SaveChangesAsync();
            }
        }

        public async Task RemoveAllMembersAsync(Guid projectId)
        {
            var members = await _context.ProjectMembers
                .Where(pm => pm.ProjectId == projectId)
                .ToListAsync();
            _context.ProjectMembers.RemoveRange(members);
            await _context.SaveChangesAsync();
        }

        public async Task<int> GetCountAsync()
        {
            return await _context.Projects.CountAsync();
        }

        public async Task<int> GetActiveCountAsync()
        {
            return await _context.Projects.CountAsync(p => p.Status == ProjectStatus.InProgress);
        }
    }
}
