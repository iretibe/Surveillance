namespace Surveillance.SharedKernel
{
    public abstract class AggregateRoot
    {
        public Guid Id { get; protected set; }

        private readonly List<IDomainEvent> _events = new();

        public IReadOnlyCollection<IDomainEvent> Events => _events;

        protected void AddDomainEvent(IDomainEvent @event)
            => _events.Add(@event);

        public void ClearEvents() => _events.Clear();
    }
}
