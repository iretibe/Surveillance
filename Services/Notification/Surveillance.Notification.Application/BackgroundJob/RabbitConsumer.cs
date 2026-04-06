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
        private readonly NotificationEventHandler _handler;
        private readonly ILogger<RabbitConsumer> _logger;

        private IConnection? _connection;
        private IChannel? _channel;

        public RabbitConsumer(
            NotificationEventHandler handler,
            ILogger<RabbitConsumer> logger)
        {
            _handler = handler;
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
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);

            try
            {
                var command = JsonSerializer.Deserialize<SendNotificationCommand>(message);

                await _handler.Handle(command!);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing message");
            }
        }
    }
}
