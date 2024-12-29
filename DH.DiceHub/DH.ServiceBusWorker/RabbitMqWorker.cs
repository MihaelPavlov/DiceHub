using DH.Messaging.Publisher;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Reflection;
using System.Text.Json;

namespace DH.ServiceBusWorker;

/// <summary>
/// Represents a RabbitMQ worker that listens to a queue and processes messages of type <typeparamref name="TMessage"/>.
/// </summary>
/// <typeparam name="TMessage">The type of message to be processed by the worker.</typeparam>
public class RabbitMqWorker<TMessage> : BackgroundService
{
    readonly IRabbitMqClient _rabbitMqClient;
    readonly IServiceProvider _serviceProvider;
    readonly string _queueName;

    /// <summary>
    /// Initializes a new instance of the <see cref="RabbitMqWorker{TMessage}"/> class.
    /// </summary>
    /// <param name="rabbitMqClient">The RabbitMQ client to interact with the message broker.</param>
    /// <param name="serviceProvider">The service provider for resolving scoped services.</param>
    /// <param name="queueName">The name of the RabbitMQ queue to consume messages from.</param>
    public RabbitMqWorker(IRabbitMqClient rabbitMqClient, IServiceProvider serviceProvider, string queueName)
    {
        _rabbitMqClient = rabbitMqClient;
        _serviceProvider = serviceProvider;
        _queueName = queueName;
    }

    /// <summary>
    /// Executes the background worker to listen for and process messages from the specified RabbitMQ queue.
    /// </summary>
    /// <param name="stoppingToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _rabbitMqClient.Consume(_queueName, async (message) =>
        {
            using var scope = _serviceProvider.CreateScope();

            var eventMessage = JsonSerializer.Deserialize<EventMessage>(message);
            if (eventMessage is null)
                throw new JsonException("Failed to deserialize the message.");

            var label = eventMessage.Label;

            var assemblyTypes = Assembly.GetAssembly(typeof(EventMessage))?.GetTypes();
            if (assemblyTypes is null)
                throw new Exception("Assembly types are null.");

            var labelType = assemblyTypes.FirstOrDefault(t => t.Name == eventMessage.Label);
            if (labelType is null)
                throw new ArgumentNullException(nameof(labelType), "Event Message Label doesn't exist.");

            var handlerType = typeof(IServiceBusHandler<>).MakeGenericType(labelType);
            var handler = scope.ServiceProvider.GetRequiredService(handlerType);

            if (handler is null)
                throw new ArgumentNullException(nameof(handlerType), $"Handler from type {handlerType.Name} was not found.");

            var messageType = typeof(EventMessage<>).MakeGenericType(labelType);
            var deserializedMessage = JsonSerializer.Deserialize(message, messageType);

            if (deserializedMessage is null)
                throw new JsonException("Failed to deserialize the message with the concrete type.");

            var method = handlerType.GetMethod(GetHandleMethodName(scope));
            if (method is not null)
            {
                // Call HandleMessageAsync dynamically
                try
                {
                    await (Task?)method.Invoke(handler, [deserializedMessage, eventMessage.MessageId, stoppingToken])!;
                }
                catch (Exception)
                {
                    throw new ArgumentNullException("Task for HandleMessageAsync dynamically failed.");
                }

            }
        });
        return Task.CompletedTask;
    }

    /// <summary>
    /// Retrieves the name of the <c>HandleMessageAsync</c> method from the 
    /// </summary>
    /// <remarks>
    /// The <c>nameof</c> operator is used to avoid hardcoding the method name 
    /// and to ensure that if the <c>HandleMessageAsync</c> method is renamed, 
    /// the compiler will produce an error, allowing the developer to update this code accordingly.
    /// This prevents runtime errors caused by hardcoded strings.
    /// </remarks>
    private string GetHandleMethodName(IServiceScope scope)
    {
        var handler = scope.ServiceProvider.GetRequiredService<IServiceBusHandler<TMessage>>();
        return nameof(handler.HandleMessageAsync);
    }

    //// TODO: Considiring other approach, but might be removed 
    //private Type? GetHandlerTypeFromLabel(string label)
    //{
    //    return label switch
    //    {
    //        "ParticipationAgreementActivated" => typeof(IServiceBusHandler<ClubActivityDetectedMessage>),
    //        "PartProcess" => typeof(IServiceBusHandler<PartProcessMessage>),
    //        // Add more cases as needed
    //        _ => null
    //    };
    //}

    //private Type? GetMessageTypeFromLabel(string label)
    //{
    //    return label switch
    //    {
    //        "ParticipationAgreementActivated" => typeof(EventMessage<ClubActivityDetectedMessage>),
    //        "PartProcess" => typeof(EventMessage<PartProcessMessage>),
    //        // Add more cases as needed
    //        _ => null
    //    };
    //}
}