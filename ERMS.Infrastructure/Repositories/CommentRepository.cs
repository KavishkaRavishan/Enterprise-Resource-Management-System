using ERMS.Application.Interfaces.Repositories;
using ERMS.Domain.Entities;
using ERMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ERMS.Infrastructure.Repositories
{
    public class CommentRepository : ICommentRepository
    {
        private readonly AppDbContext _context;

        public CommentRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<TaskComment?> GetByIdAsync(Guid id)
        {
            return await _context.TaskComments
                .Include(c => c.Author)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<List<TaskComment>> GetByTaskIdAsync(Guid taskId)
        {
            return await _context.TaskComments
                .Include(c => c.Author)
                .Where(c => c.TaskId == taskId)
                .OrderByDescending(c => c.Created)
                .ToListAsync();
        }

        public async Task AddAsync(TaskComment comment)
        {
            await _context.TaskComments.AddAsync(comment);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var comment = await _context.TaskComments.FindAsync(id);
            if (comment != null)
            {
                _context.TaskComments.Remove(comment);
                await _context.SaveChangesAsync();
            }
        }
    }
}
