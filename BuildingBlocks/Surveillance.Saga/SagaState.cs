namespace Surveillance.Saga
{
    public class SagaState
    {
        public Guid Id { get; set; }
        public string CurrentStep { get; set; } = string.Empty;
        public bool IsCompleted { get; set; }
        public bool IsFailed { get; set; }
    }
}
