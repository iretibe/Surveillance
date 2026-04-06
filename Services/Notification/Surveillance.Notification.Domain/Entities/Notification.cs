using Surveillance.SharedKernel;

namespace Surveillance.Notification.Domain.Entities
{
    public class Notification : AggregateRoot
    {
        public Guid AlertId { get; private set; }
        public string Message { get; private set; } = string.Empty;
        public DateTime CreatedAt { get; private set; }
        public bool IsSent { get; private set; }

        private Notification() { }

        public static Notification Create(Guid alertId, string message)
        {
            return new Notification
            {
                Id = Guid.NewGuid(),
                AlertId = alertId,
                Message = message,
                CreatedAt = DateTime.UtcNow,
                IsSent = false
            };
        }

        public void MarkAsSent()
        {
            IsSent = true;
        }
    }
}
