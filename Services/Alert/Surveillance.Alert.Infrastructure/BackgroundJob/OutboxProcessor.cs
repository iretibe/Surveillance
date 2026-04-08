using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<OutboxProcessor> _logger;

        public OutboxProcessor(
            IServiceScopeFactory scopeFactory,
            ILogger<OutboxProcessor> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("OutboxProcessor started");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _scopeFactory.CreateScope())
                    {
                        var db = scope.ServiceProvider.GetRequiredService<AlertDbContext>();
                        var bus = scope.ServiceProvider.GetRequiredService<IEventBus>();
                        var dispatcher = scope.ServiceProvider.GetRequiredService<SagaDispatcher>();

                        // Get unprocessed messages
                        var messages = await db.OutboxMessages
                            .Where(x => x.ProcessedOn == null)
                            .OrderBy(x => x.OccurredOn)
                            .Take(10)
                            .ToListAsync(stoppingToken);

                        if (messages.Any())
                        {
                            _logger.LogInformation("Processing {Count} outbox messages", messages.Count);
                        }

                        foreach (var msg in messages)
                        {
                            try
                            {
                                var eventId = msg.Id;

                                // Check for duplicate processing
                                if (await db.ProcessedEvents.AnyAsync(x => x.Id == eventId, stoppingToken))
                                {
                                    _logger.LogWarning("Event {EventId} already processed, skipping", eventId);
                                    msg.ProcessedOn = DateTime.UtcNow;
                                    continue;
                                }

                                // Deserialize the event
                                var eventType = Type.GetType(msg.Type) ?? typeof(object);
                                var @event = JsonSerializer.Deserialize(msg.Payload, eventType, new JsonSerializerOptions
                                {
                                    PropertyNameCaseInsensitive = true
                                });

                                if (@event == null)
                                {
                                    _logger.LogError("Failed to deserialize event {EventId} of type {EventType}", eventId, msg.Type);
                                    msg.ProcessedOn = DateTime.UtcNow;
                                    msg.Error = "Deserialization failed";
                                    continue;
                                }

                                // Step 1: Dispatch to Saga
                                _logger.LogDebug("Dispatching event {EventId} to saga", eventId);
                                await dispatcher.DispatchAsync(@event);

                                // Step 2: Publish to EventBus with retry policy
                                var retryPolicy = Policy
                                    .Handle<Exception>()
                                    .WaitAndRetryAsync(
                                        3,
                                        retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                                        onRetry: (exception, timeSpan, retryCount, context) =>
                                        {
                                            _logger.LogWarning(exception,
                                                "Retry {RetryCount} for publishing event {EventId} after {Delay}s",
                                                retryCount, eventId, timeSpan.TotalSeconds);
                                        });

                                using var cts = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken);
                                cts.CancelAfter(TimeSpan.FromSeconds(10));

                                await retryPolicy.ExecuteAsync(async () =>
                                {
                                    await bus.PublishAsync(@event, cts.Token);
                                });

                                // Mark as processed
                                db.ProcessedEvents.Add(new ProcessedEvent
                                {
                                    Id = eventId,
                                    ProcessedAt = DateTime.UtcNow
                                });

                                msg.ProcessedOn = DateTime.UtcNow;
                                msg.Error = null;

                                _logger.LogInformation("Successfully processed event {EventId}", eventId);
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex, "Error processing message {MessageId}", msg.Id);
                                msg.ProcessedOn = DateTime.UtcNow;
                                msg.Error = ex.Message;

                                // Optional: Increment retry count
                                msg.RetryCount = (msg.RetryCount ?? 0) + 1;

                                // If max retries reached, move to dead letter
                                if (msg.RetryCount >= 3)
                                {
                                    _logger.LogError("Message {MessageId} moved to dead letter after {RetryCount} retries",
                                        msg.Id, msg.RetryCount);
                                    msg.IsDeadLetter = true;
                                }
                            }
                        }

                        if (messages.Any())
                        {
                            await db.SaveChangesAsync(stoppingToken);
                            _logger.LogInformation("Saved {Count} processed messages", messages.Count);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in OutboxProcessor main loop");
                }

                // Wait before next iteration
                await Task.Delay(2000, stoppingToken);
            }

            _logger.LogInformation("OutboxProcessor stopped");
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("OutboxProcessor is stopping");
            await base.StopAsync(cancellationToken);
        }
    }
}