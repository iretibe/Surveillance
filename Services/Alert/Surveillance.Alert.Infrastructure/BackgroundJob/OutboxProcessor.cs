using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Polly;
using Surveillance.Alert.Domain.Events.Entities;
using Surveillance.Alert.Infrastructure.Data;
using Surveillance.EventBus.Events;
using Surveillance.Saga;
using System.Text.Json;

namespace Surveillance.Alert.Infrastructure.BackgroundJob
{
    public class OutboxProcessor : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public OutboxProcessor(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _scopeFactory.CreateScope();

                var db = scope.ServiceProvider.GetRequiredService<AlertDbContext>();
                var bus = scope.ServiceProvider.GetRequiredService<IEventBus>();

                var dispatcher = scope.ServiceProvider.GetRequiredService<SagaDispatcher>();

                var messages = await db.OutboxMessages
                    .Where(x => x.ProcessedOn == null)
                    .Take(10)
                    .ToListAsync(stoppingToken);

                //foreach (var msg in messages)
                //{
                //    await bus.PublishAsync(msg.Payload);
                //    msg.ProcessedOn = DateTime.UtcNow;
                //}

                //foreach (var msg in messages)
                //{
                //    var @event = JsonSerializer.Deserialize<object>(msg.Payload);

                //    await dispatcher.DispatchAsync(@event!);

                //    await bus.PublishAsync(msg.Payload);

                //    msg.ProcessedOn = DateTime.UtcNow;
                //}

                foreach (var msg in messages)
                {
                    var eventId = msg.Id;

                    if (await db.ProcessedEvents.AnyAsync(x => x.Id == eventId))
                        continue;

                    var @event = JsonSerializer.Deserialize<object>(msg.Payload);

                    await dispatcher.DispatchAsync(@event!);

                    var retry = Policy
                        .Handle<Exception>()
                        .WaitAndRetryAsync(3, _ => TimeSpan.FromSeconds(2));

                    using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));

                    await retry.ExecuteAsync(() =>
                        bus.PublishAsync(msg.Payload).WaitAsync(cts.Token));

                    db.ProcessedEvents.Add(new ProcessedEvent
                    {
                        Id = eventId,
                        ProcessedAt = DateTime.UtcNow
                    });

                    msg.ProcessedOn = DateTime.UtcNow;
                }

                await db.SaveChangesAsync(stoppingToken);

                await Task.Delay(2000, stoppingToken);
            }
        }
    }
}
