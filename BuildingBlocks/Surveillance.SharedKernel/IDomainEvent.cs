namespace Surveillance.SharedKernel
{
    public interface IDomainEvent
    {
        DateTime OccurredOn { get; }
    }
}
