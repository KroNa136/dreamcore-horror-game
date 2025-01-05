using DreamcoreHorrorGameStatisticsServer.Models.DTO;
using DreamcoreHorrorGameStatisticsServer.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Text;

namespace DreamcoreHorrorGameStatisticsServer.Services;

public class RequestsBackgroundController
(
    IServiceProvider serviceProvider,
    ILogger<RequestsBackgroundController> logger,
    IRabbitMqConsumer rabbitMqConsumer,
    IJsonSerializerOptionsProvider jsonSerializerOptionsProvider
)
: IHostedService
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;
    private readonly ILogger<RequestsBackgroundController> _logger = logger;

    private readonly IRabbitMqConsumer _rabbitMqConsumer = rabbitMqConsumer;
    private readonly IJsonSerializerOptionsProvider _jsonSerializerOptionsProvider = jsonSerializerOptionsProvider;

    public Task StartAsync(CancellationToken cancellationToken)
    {
        ConfigureConsumer(cancellationToken);
        return Task.CompletedTask;
    }

    private async Task ConfigureConsumer(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Initiated RabbitMQ consumer configuration for {type}", GetType().Name);

        await _rabbitMqConsumer.StartConsumingFromAsync(exchange: "statistics", queue: "statistics-server");
        _rabbitMqConsumer.ReceivedMessage += CreateFromBytesAsync;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _rabbitMqConsumer.Dispose();
        return Task.CompletedTask;
    }

    private async void CreateFromBytesAsync(byte[] data)
    {
        _logger.LogInformation("CreateFromBytes was called for Request.");

        var context = _serviceProvider.GetRequiredService<DreamcoreHorrorGameStatisticsContext>();

        string dataString = Encoding.UTF8.GetString(data);

        try
        {
            RequestDTO? requestDTO = JsonSerializer.Deserialize<RequestDTO>(dataString, _jsonSerializerOptionsProvider.Default);

            if (requestDTO is null)
                return;

            if (string.IsNullOrEmpty(requestDTO.SenderName) ||
                string.IsNullOrEmpty(requestDTO.ControllerName) ||
                string.IsNullOrEmpty(requestDTO.MethodName))
                return;

            var sender = await context.Senders
                .FirstOrDefaultAsync(sender => sender.Name.Equals(requestDTO.SenderName));

            if (sender is null)
            {
                sender = new()
                {
                    Id = Guid.NewGuid(),
                    Name = requestDTO.SenderName
                };

                context.Senders.Add(sender);
                await context.SaveChangesAsync();
            }

            var controller = await context.Controllers
                .FirstOrDefaultAsync(controller => controller.Name.Equals(requestDTO.ControllerName));

            if (controller is null)
            {
                controller = new()
                {
                    Id = Guid.NewGuid(),
                    Name = requestDTO.ControllerName
                };

                context.Controllers.Add(controller);
                await context.SaveChangesAsync();
            }

            var method = await context.Methods
                .FirstOrDefaultAsync(method => method.Name.Equals(requestDTO.MethodName));

            if (method is null)
            {
                method = new()
                {
                    Id = Guid.NewGuid(),
                    Name = requestDTO.MethodName
                };

                context.Methods.Add(method);
                await context.SaveChangesAsync();
            }

            Request request = new()
            {
                Id = Guid.NewGuid(),
                ReceptionTimestamp = requestDTO.ReceptionTimestamp.ToUniversalTime(),
                SenderId = Guid.Empty,
                ControllerId = Guid.Empty,
                MethodId = Guid.Empty,
                Sender = sender,
                Controller = controller,
                Method = method
            };

            context.Requests.Add(request);
            await context.SaveChangesAsync();
        }
        catch (JsonException ex)
        {
            _logger.LogError
            (
                eventId: new EventId("CreateFromBytesAsync".GetHashCode() + ex.GetType().GetHashCode()),
                message: "JSON deserialization error occured while creating Request from bytes.{newline}{message}", Environment.NewLine, ex.Message
            );
        }
        catch (DbUpdateConcurrencyException ex)
        {
            _logger.LogError
            (
                eventId: new EventId("CreateFromBytesAsync".GetHashCode() + ex.GetType().GetHashCode()),
                message: "Database conflict occured while creating Request from bytes."
            );
        }
    }
}
