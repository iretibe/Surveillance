using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace Surveillance.EventBus.Events
{
    public class RabbitMqEventBus : IEventBus
    {
        private readonly ILogger<RabbitMqEventBus> _logger;

        private IConnection? _connection;
        private IChannel? _channel;

        public RabbitMqEventBus(ILogger<RabbitMqEventBus> logger)
        {
            _logger = logger;
        }

        public async Task PublishAsync<T>(T @event, CancellationToken cancellationToken)
        {
            var factory = new ConnectionFactory()
            {
                HostName = "rabbitmq",
                //HostName = _options.HostName,
                //Port = _options.Port,
                //UserName = _options.UserName,
                //Password = _options.Password
            };

            _connection = await factory.CreateConnectionAsync();
            _channel = await _connection.CreateChannelAsync();

            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(@event));

            await _channel.BasicPublishAsync(
                exchange: "",
                routingKey: "alerts",
                body: body
            );
        }
    }
}
