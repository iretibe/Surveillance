namespace Surveillance.Saga
{
    public interface ISaga
    {
        Task HandleAsync(object @event, CancellationToken cancellationToken = default);
    }
}
