using Surveillance.Notification.Domain.Repositories;
using Surveillance.Notification.Infrastructure.Data;

namespace Surveillance.Notification.Infrastructure.Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly NotificationDbContext _db;

        public NotificationRepository(NotificationDbContext db)
        {
            _db = db;
        }

        public async Task AddAsync(Domain.Entities.Notification notification)
        {
            await _db.Notifications.AddAsync(notification);
        }

        public async Task SaveAsync()
        {
            await _db.SaveChangesAsync();
        }
    }
}
