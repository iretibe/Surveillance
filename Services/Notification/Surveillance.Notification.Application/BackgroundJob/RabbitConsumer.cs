using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Surveillance.EventBus.Events.Notifications;
using Surveillance.Notification.Application.EventHandlers;
using System.Text;
using System.Text.Json;

namespace Surveillance.Notification.Application.BackgroundJob
{
    public class RabbitConsumer : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<RabbitConsumer> _logger;

        private IConnection? _connection;
        private IChannel? _channel;

        public RabbitConsumer(
            IServiceScopeFactory scopeFactory,
            ILogger<RabbitConsumer> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var factory = new ConnectionFactory()
            {
                HostName = "rabbitmq"
            };

            _connection = await factory.CreateConnectionAsync(stoppingToken);
            _channel = await _connection.CreateChannelAsync();

            await _channel.QueueDeclareAsync("alerts", false, false, false);

            var consumer = new AsyncEventingBasicConsumer(_channel);

            consumer.ReceivedAsync += HandleMessageAsync;

            await _channel.BasicConsumeAsync("alerts", true, consumer);

            _logger.LogInformation("Subscribed to alerts queue");
        }

        private async Task HandleMessageAsync(object sender, BasicDeliverEventArgs ea)
        {
            // Create a scope for each message to resolve scoped services
            using (var scope = _scopeFactory.CreateScope())
            {
                var handler = scope.ServiceProvider.GetRequiredService<NotificationEventHandler>();

                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                try
                {
                    var command = JsonSerializer.Deserialize<SendNotificationCommand>(message);
                    await handler.Handle(command!);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing message");
                }
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            if (_channel != null)
                await _channel.CloseAsync();

            if (_connection != null)
                await _connection.CloseAsync();

            await base.StopAsync(cancellationToken);
        }
    }
}