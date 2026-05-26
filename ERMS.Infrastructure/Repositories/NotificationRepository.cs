using ERMS.Application.Interfaces.Repositories;
using ERMS.Domain.Entities;
using ERMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ERMS.Infrastructure.Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly AppDbContext _context;

        public NotificationRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Notification?> GetByIdAsync(Guid id)
        {
            return await _context.Set<Notification>().FindAsync(id);
        }

        public async Task<List<Notification>> GetByUserIdAsync(Guid userId)
        {
            return await _context.Set<Notification>()
                .Where(n => n.RecipientId == userId)
                .ToListAsync();
        }

        public async Task AddAsync(Notification notification)
        {
            await _context.Set<Notification>().AddAsync(notification);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Notification notification)
        {
            _context.Set<Notification>().Update(notification);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateRangeAsync(List<Notification> notifications)
        {
            _context.Set<Notification>().UpdateRange(notifications);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var notification = await GetByIdAsync(id);
            if (notification != null)
            {
                _context.Set<Notification>().Remove(notification);
                await _context.SaveChangesAsync();
            }
        }
    }
}
