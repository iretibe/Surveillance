namespace Surveillance.Alert.Domain.Dtos
{
    public class AlertDto
    {
        public Guid Id { get; set; }
        public string? Message { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid UserId { get; set; }
    }
}
