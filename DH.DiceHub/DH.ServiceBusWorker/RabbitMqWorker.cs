using DH.Messaging.Publisher;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Text.Json;

namespace DH.ServiceBusWorker;

public class RabbitMqWorker<TMessage> : BackgroundService
{
    private readonly IRabbitMqClient _rabbitMqClient;
    private readonly IServiceProvider _serviceProvider;
    private readonly string _queueName;

    public RabbitMqWorker(IRabbitMqClient rabbitMqClient, IServiceProvider serviceProvider, string queueName)
    {
        _rabbitMqClient = rabbitMqClient;
        _serviceProvider = serviceProvider;
        _queueName = queueName;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _rabbitMqClient.Consume(_queueName, async (message) =>
        {
            using var scope = _serviceProvider.CreateScope();
            var handler = scope.ServiceProvider.GetRequiredService<IServiceBusHandler<TMessage>>();
            var deserializedMessage = JsonSerializer.Deserialize<EventMessage<TMessage>>(message);
            await handler.HandleMessageAsync(deserializedMessage, deserializedMessage.MessageId, stoppingToken);
        });

        return Task.CompletedTask;
    }
}