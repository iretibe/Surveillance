using Surveillance.SharedKernel;

namespace Surveillance.Alert.Domain.Events
{
    public record AlertCreatedEvent(Guid Id, string Message, Guid UserId) 
        : IDomainEvent
    {
        public DateTime OccurredOn => DateTime.UtcNow;
    }
}
