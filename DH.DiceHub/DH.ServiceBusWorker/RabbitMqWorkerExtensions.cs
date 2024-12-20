using DH.Messaging.Publisher;
using Microsoft.Extensions.DependencyInjection;

namespace DH.ServiceBusWorker;

public static class RabbitMqWorkerExtensions
{
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
