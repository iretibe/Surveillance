namespace Surveillance.EventBus.Events.Notifications
{
    public record SendNotificationCommand(Guid AlertId, string Message);
}
