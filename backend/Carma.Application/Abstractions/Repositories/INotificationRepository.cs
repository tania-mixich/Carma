using Carma.Domain.Entities;

namespace Carma.Application.Abstractions.Repositories;

public interface INotificationRepository
{
    Task<IEnumerable<Notification>> GetAllForUserAsync(Guid userId);
    Task<IEnumerable<Notification>> GetUnreadForUserAsync(Guid userId);
    Task<Notification?> GetByIdAsync(int id);
    Task<Notification> CreateAsync(Notification notification);
    Task AddRangeAsync(IEnumerable<Notification> notifications);
    void Update(Notification notification);
}