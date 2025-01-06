using DreamcoreHorrorGameStatisticsServer.ConstantValues;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using System.Text;

namespace DreamcoreHorrorGameStatisticsServer.Services;

public class RabbitMqConsumer : IRabbitMqConsumer
{
    public event Action<byte[]> ReceivedMessage = (message) => { };

    private readonly IConfiguration _configuration;
    private readonly ILogger<IRabbitMqConsumer> _logger;

    private readonly ConnectionFactory _connectionFactory;

    private IConnection _connection = null!;
    private IChannel _channel = null!;
    private AsyncEventingBasicConsumer _consumer = null!;

    protected readonly Func<string, Exception?, string> _customLoggingFormatter = (message, exception) =>
    {
        StringBuilder sb = new(message);

        if (exception is not null)
        {
            sb.Append($"{Environment.NewLine}{exception.GetType()}: {exception.Message}");

            if (!string.IsNullOrEmpty(exception.StackTrace))
                sb.Append($"{Environment.NewLine}{exception.StackTrace}");
        }

        return sb.ToString();
    };

    public RabbitMqConsumer(IConfiguration configuration, ILogger<IRabbitMqConsumer> logger)
    {
        _configuration = configuration;
        _logger = logger;

        string? userName = _configuration.GetSection(ConfigurationProperties.RabbitMQ)[ConfigurationProperties.RabbitMqUserName];
        string? password = _configuration.GetSection(ConfigurationProperties.RabbitMQ)[ConfigurationProperties.RabbitMqPassword];
        string? hostName = _configuration.GetSection(ConfigurationProperties.RabbitMQ)[ConfigurationProperties.RabbitMqHostName];
        string? portStr = _configuration.GetSection(ConfigurationProperties.RabbitMQ)[ConfigurationProperties.RabbitMqPort];

        if (string.IsNullOrEmpty(userName))
            userName = "guest";

        if (string.IsNullOrEmpty(password))
            password = "guest";

        if (string.IsNullOrEmpty(hostName))
            hostName = "localhost";

        if (string.IsNullOrEmpty(portStr))
            portStr = "5672";

        if (!int.TryParse(portStr, out int port))
            port = 5672;

        _connectionFactory = new()
        {
            UserName = userName,
            Password = password,
            HostName = hostName,
            Port = port
        };
    }

    public async Task StartConsumingFromAsync(string exchange, string queue)
    {
        try
        {
            _connection ??= await _connectionFactory.CreateConnectionAsync();
            _channel ??= await _connection.CreateChannelAsync();

            await _channel.ExchangeDeclareAsync(exchange: exchange, type: ExchangeType.Fanout);
            await _channel.QueueDeclareAsync(queue: queue, durable: true, exclusive: false, autoDelete: false, arguments: null);
            await _channel.QueueBindAsync(queue: queue, exchange: exchange, routingKey: string.Empty);

            _consumer = new AsyncEventingBasicConsumer(_channel);

            _consumer.ReceivedAsync += async (channel, eventArgs) =>
            {
                byte[] body = eventArgs.Body.ToArray();
                ReceivedMessage.Invoke(body);

                await _channel!.BasicAckAsync(deliveryTag: eventArgs.DeliveryTag, multiple: false);
            };

            await _channel.BasicConsumeAsync(queue: queue, autoAck: false, consumer: _consumer);
        }
        catch (BrokerUnreachableException ex)
        {
            _logger.Log
            (
                logLevel: LogLevel.Error,
                eventId: new EventId("StartConsumingFromAsync".GetHashCode() + ex.GetType().GetHashCode()),
                exception: ex,
                state: "Failed to open a connection to RabbitMQ.",
                formatter: _customLoggingFormatter
            );
        }
    }

    public async void Dispose()
    {
        if (_channel is not null)
        {
            await _channel.CloseAsync();
            await _channel.DisposeAsync();
        }

        if (_connection is not null)
        {
            await _connection.CloseAsync();
            await _connection.DisposeAsync();
        }

        GC.SuppressFinalize(this);
    }
}
