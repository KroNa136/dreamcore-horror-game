namespace DreamcoreHorrorGameApiServer.Services;

public interface IRabbitMqProducer : IDisposable
{
    public Task<bool> PublishMessageAsync(string exchange, ReadOnlyMemory<byte> message);
}
