namespace Surveillance.Alert.Domain.Saga
{
    public class SagaStateEntity
    {
        public Guid Id { get; set; }
        public string CurrentStep { get; set; } = string.Empty;
        public bool IsCompleted { get; set; }
        public bool IsFailed { get; set; }
    }
}
