using Microsoft.Extensions.Logging;
using Surveillance.EventBus.Events;
using Surveillance.EventBus.Events.Notifications;
using Surveillance.Notification.Domain.Repositories;

namespace Surveillance.Notification.Application.EventHandlers
{
    public class NotificationEventHandler
    {
        private readonly INotificationRepository _repo;
        private readonly IEventBus _bus;
        private readonly ILogger<NotificationEventHandler> _logger;

        public NotificationEventHandler(
            INotificationRepository repo,
            IEventBus bus,
            ILogger<NotificationEventHandler> logger)
        {
            _repo = repo;
            _bus = bus;
            _logger = logger;
        }

        public async Task Handle(SendNotificationCommand cmd)
        {
            try
            {
                var notification = Domain.Entities.Notification.Create(cmd.AlertId, cmd.Message);

                await _repo.AddAsync(notification);

                // simulate sending
                _logger.LogInformation($"Notification sent: {cmd.Message}");

                notification.MarkAsSent();

                await _repo.SaveAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Notification failed");

                await _bus.PublishAsync(new NotificationFailedEvent(cmd.AlertId));
            }
        }
    }
}
