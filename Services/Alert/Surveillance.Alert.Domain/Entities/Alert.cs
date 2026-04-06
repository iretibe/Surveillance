using Surveillance.Alert.Domain.Events;
using Surveillance.SharedKernel;

namespace Surveillance.Alert.Domain.Entities
{
    public class Alert : AggregateRoot
    {
        public string Message { get; private set; }
        public DateTime CreatedAt { get; private set; }

        private Alert() { }

        public static Alert Create(string message)
        {
            var alert = new Alert
            {
                Id = Guid.NewGuid(),
                Message = message,
                CreatedAt = DateTime.UtcNow
            };

            alert.AddDomainEvent(new AlertCreatedEvent(alert.Id, message));

            return alert;
        }
    }
}
