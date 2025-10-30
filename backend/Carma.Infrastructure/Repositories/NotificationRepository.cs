using Carma.Application.Abstractions.Repositories;
using Carma.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Carma.Infrastructure.Repositories;

public class NotificationRepository : INotificationRepository
{
    private readonly CarmaDbContext _context;
    
    public NotificationRepository(CarmaDbContext context)
    {
        _context = context;
    }


    public async Task<IEnumerable<Notification>> GetAllForUserAsync(Guid userId)
    {
        var notifications = await _context.Notifications
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.SentAt)
            .ToListAsync();
        return notifications;
    }

    public async Task<IEnumerable<Notification>> GetUnreadForUserAsync(Guid userId)
    {
        var notifications = await _context.Notifications
            .Where(n => n.UserId == userId && n.IsRead == false)
            .OrderByDescending(n => n.SentAt)
            .ToListAsync();
        return notifications;
    }
    
    public async Task<Notification?> GetByIdAsync(int id)
    {
        return await _context.Notifications.FindAsync(id);
    }

    public async Task<Notification> CreateAsync(Notification notification)
    {
        await _context.Notifications.AddAsync(notification);
        return notification;
    }
    
    public async Task AddRangeAsync(IEnumerable<Notification> notifications)
    {
        await _context.Notifications.AddRangeAsync(notifications);
    }


    public void Update(Notification notification)
    {
        _context.Notifications.Update(notification);
    }
}