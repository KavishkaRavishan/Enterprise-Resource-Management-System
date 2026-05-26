using ERMS.Application.Interfaces.Repositories;
using ERMS.Domain.Entities;
using ERMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ERMS.Infrastructure.Repositories
{
    public class TimeLogRepository : ITimeLogRepository
    {
        private readonly AppDbContext _context;

        public TimeLogRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<TimeLog?> GetByIdAsync(Guid id)
        {
            return await _context.Set<TimeLog>()
                .Include(t => t.User)
                .Include(t => t.Task)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<List<TimeLog>> GetByTaskIdAsync(Guid taskId)
        {
            return await _context.Set<TimeLog>()
                .Include(t => t.User)
                .Where(t => t.TaskId == taskId)
                .OrderByDescending(t => t.DateLogged)
                .ToListAsync();
        }

        public async Task<List<TimeLog>> GetByUserIdAsync(Guid userId)
        {
            return await _context.Set<TimeLog>()
                .Include(t => t.Task)
                .Where(t => t.UserId == userId)
                .OrderByDescending(t => t.DateLogged)
                .ToListAsync();
        }

        public async Task<List<TimeLog>> GetByProjectIdAsync(Guid projectId)
        {
            return await _context.Set<TimeLog>()
                .Include(t => t.User)
                .Include(t => t.Task)
                .Where(t => t.Task.ProjectId == projectId)
                .OrderByDescending(t => t.DateLogged)
                .ToListAsync();
        }

        public async Task AddAsync(TimeLog timeLog)
        {
            await _context.Set<TimeLog>().AddAsync(timeLog);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(TimeLog timeLog)
        {
            _context.Set<TimeLog>().Update(timeLog);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var timeLog = await _context.Set<TimeLog>().FindAsync(id);
            if (timeLog != null)
            {
                _context.Set<TimeLog>().Remove(timeLog);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<decimal> GetTotalHoursByProjectAsync(Guid projectId)
        {
            return await _context.Set<TimeLog>()
                .Where(t => t.Task.ProjectId == projectId)
                .SumAsync(t => t.HoursSpent);
        }
    }
}
