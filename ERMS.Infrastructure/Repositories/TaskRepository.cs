using ERMS.Application.Interfaces.Repositories;
using ERMS.Domain.Entities;
using ERMS.Domain.Enums;
using ERMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ERMS.Infrastructure.Repositories
{
    public class TaskRepository : ITaskRepository
    {
        private readonly AppDbContext _context;

        public TaskRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ProjectTask?> GetByIdAsync(Guid id)
        {
            return await _context.ProjectTasks.FindAsync(id);
        }

        public async Task<ProjectTask?> GetByIdWithDetailsAsync(Guid id)
        {
            return await _context.ProjectTasks
                .Include(t => t.Project)
                .Include(t => t.AssignedTo)
                .Include(t => t.Comments)
                    .ThenInclude(c => c.Author)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<List<ProjectTask>> GetByProjectIdAsync(Guid projectId)
        {
            return await _context.ProjectTasks
                .Include(t => t.Project)
                .Include(t => t.AssignedTo)
                .Include(t => t.Comments)
                .Where(t => t.ProjectId == projectId)
                .OrderBy(t => t.Status)
                .ThenByDescending(t => t.Priority)
                .ToListAsync();
        }

        public async Task<List<ProjectTask>> GetByAssignedUserIdAsync(Guid userId)
        {
            return await _context.ProjectTasks
                .Include(t => t.Project)
                .Include(t => t.AssignedTo)
                .Include(t => t.Comments)
                .Where(t => t.AssignedToId == userId)
                .OrderBy(t => t.Status)
                .ToListAsync();
        }

        public async Task AddAsync(ProjectTask task)
        {
            await _context.ProjectTasks.AddAsync(task);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(ProjectTask task)
        {
            _context.ProjectTasks.Update(task);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var task = await _context.ProjectTasks.FindAsync(id);
            if (task != null)
            {
                _context.ProjectTasks.Remove(task);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<int> GetCountAsync()
        {
            return await _context.ProjectTasks.CountAsync();
        }

        public async Task<int> GetCountByStatusAsync(TaskItemStatus status)
        {
            return await _context.ProjectTasks.CountAsync(t => t.Status == status);
        }
    }
}
