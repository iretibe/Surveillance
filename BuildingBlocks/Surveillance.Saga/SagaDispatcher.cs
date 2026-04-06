namespace Surveillance.Saga
{
    public class SagaDispatcher
    {
        private readonly IEnumerable<ISaga> _sagas;

        public SagaDispatcher(IEnumerable<ISaga> sagas)
        {
            _sagas = sagas;
        }

        public async Task DispatchAsync(object @event)
        {
            foreach (var saga in _sagas)
            {
                await saga.HandleAsync(@event);
            }
        }
    }
}
