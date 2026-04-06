using Microsoft.EntityFrameworkCore;
using Surveillance.Alert.Domain.Entities;
using Surveillance.Alert.Domain.Events.Entities;
using Surveillance.Alert.Domain.Saga;
using Surveillance.SharedKernel;
using System.Text.Json;

namespace Surveillance.Alert.Infrastructure.Data
{
    public class AlertDbContext : DbContext, IUnitOfWork
    {
        public AlertDbContext(DbContextOptions<AlertDbContext> options)
            : base(options) { }

        public DbSet<Domain.Entities.Alert> Alerts => Set<Domain.Entities.Alert>();
        public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();
        public DbSet<SagaStateEntity> SagaStates => Set<SagaStateEntity>();
        public DbSet<ProcessedEvent> ProcessedEvents => Set<ProcessedEvent>();

        public override async Task<int> SaveChangesAsync(CancellationToken ct = default)
        {
            var domainEvents = ChangeTracker
                .Entries<AggregateRoot>()
                .SelectMany(e => e.Entity.Events)
                .ToList();

            foreach (var @event in domainEvents)
            {
                OutboxMessages.Add(new OutboxMessage
                {
                    Id = Guid.NewGuid(),
                    Type = @event.GetType().Name,
                    Payload = JsonSerializer.Serialize(@event),
                    OccurredOn = DateTime.UtcNow
                });
            }

            return await base.SaveChangesAsync(ct);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.HasDefaultSchema("alert");

            builder.Entity<Domain.Entities.Alert>(e =>
            {
                e.HasKey(x => x.Id);
                e.Property(x => x.Message).IsRequired();
            });

            base.OnModelCreating(builder);
        }
    }
}
