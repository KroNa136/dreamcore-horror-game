namespace DreamcoreHorrorGameEmailServer.Services;

public interface IRabbitMqConsumer : IDisposable
{
    public event Action<byte[]> ReceivedMessage;

    public Task StartConsumingFromAsync(string exchange, string queue);
}
