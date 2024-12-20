using DH.Messaging.Publisher;

using Microsoft.Extensions.DependencyInjection;

namespace DH.ServiceBusWorker;

/// <summary>
/// Provides extension methods for registering RabbitMQ workers in the dependency injection container.
/// </summary>
public static class RabbitMqWorkerExtensions
{
    /// <summary>
    /// Registers a scoped RabbitMQ worker and its corresponding message handler in the service collection.
    /// </summary>
    /// <typeparam name="TMessage">The type of the message the worker processes.</typeparam>
    /// <typeparam name="THandler">The handler type that implements <see cref="IServiceBusHandler{TMessage}"/> to process the messages.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the worker and handler to.</param>
    /// <param name="queueName">The name of the RabbitMQ queue the worker will consume messages from.</param>
    /// <returns>The updated <see cref="IServiceCollection"/> with the RabbitMQ worker and handler registered.</returns>
    public static IServiceCollection AddScopedRabbitMqWorker<TMessage, THandler>(
        this IServiceCollection services, string queueName)
        where THandler : class, IServiceBusHandler<TMessage>
    {
        services.AddScoped<IServiceBusHandler<TMessage>, THandler>();
        services.AddHostedService(sp =>
        {
            var rabbitMqClient = sp.GetRequiredService<IRabbitMqClient>();
            return new RabbitMqWorker<TMessage>(rabbitMqClient, sp, queueName);
        });

        return services;
    }
}
