using DreamcoreHorrorGameApiServer.ConstantValues;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
using System.Text;

namespace DreamcoreHorrorGameApiServer.Services;

public class RabbitMqProducer : IRabbitMqProducer
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<IRabbitMqProducer> _logger;

    private readonly ConnectionFactory _connectionFactory;

    private IConnection _connection = null!;
    private IChannel _channel = null!;

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

    public RabbitMqProducer(IConfiguration configuration, ILogger<IRabbitMqProducer> logger)
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

    public async Task<bool> PublishMessageAsync(string exchange, ReadOnlyMemory<byte> message)
    {
        try
        {
            _connection ??= await _connectionFactory.CreateConnectionAsync();
            _channel ??= await _connection.CreateChannelAsync();

            await _channel.ExchangeDeclareAsync(exchange: exchange, type: ExchangeType.Fanout);

            await _channel.BasicPublishAsync(exchange: exchange, routingKey: string.Empty, body: message);

            return true;
        }
        catch (BrokerUnreachableException ex)
        {
            _logger.Log
            (
                logLevel: LogLevel.Error,
                eventId: new EventId("PublishMessageAsync".GetHashCode() + ex.GetType().GetHashCode()),
                exception: ex,
                state: "Failed to open a connection to RabbitMQ.",
                formatter: _customLoggingFormatter
            );

            return false;
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
