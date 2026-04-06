namespace Surveillance.Alert.Domain.Events.Entities
{
    // IDEMPOTENCY
    public class ProcessedEvent
    {
        public Guid Id { get; set; }
        public DateTime ProcessedAt { get; set; }
    }
}
