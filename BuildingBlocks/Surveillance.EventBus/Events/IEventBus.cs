namespace Surveillance.EventBus.Events
{
    public interface IEventBus
    {
        Task PublishAsync<T>(T @event);
    }
}
