using ERMS.Application.Interfaces.Repositories;
using ERMS.Domain.Entities;
using ERMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ERMS.Infrastructure.Repositories
{
    public class AttachmentRepository : IAttachmentRepository
    {
        private readonly AppDbContext _context;

        public AttachmentRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Attachment?> GetByIdAsync(Guid id)
        {
            return await _context.Set<Attachment>()
                .Include(a => a.UploadedBy)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<List<Attachment>> GetByTaskIdAsync(Guid taskId)
        {
            return await _context.Set<Attachment>()
                .Include(a => a.UploadedBy)
                .Where(a => a.TaskId == taskId)
                .OrderByDescending(a => a.Created)
                .ToListAsync();
        }

        public async Task AddAsync(Attachment attachment)
        {
            await _context.Set<Attachment>().AddAsync(attachment);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var attachment = await _context.Set<Attachment>().FindAsync(id);
            if (attachment != null)
            {
                _context.Set<Attachment>().Remove(attachment);
                await _context.SaveChangesAsync();
            }
        }
    }
}
