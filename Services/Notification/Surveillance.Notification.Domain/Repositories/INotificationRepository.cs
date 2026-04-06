namespace Surveillance.Notification.Domain.Repositories
{
    public interface INotificationRepository
    {
        Task AddAsync(Entities.Notification notification);
        Task SaveAsync();
    }
}
