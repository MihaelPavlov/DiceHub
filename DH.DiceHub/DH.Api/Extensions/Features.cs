using DH.Application.Services;
using DH.Domain.Services.Publisher;
using DH.Messaging.Publisher.Authentication;
using DH.Messaging.Publisher;
using DH.Domain.Models.Common;
using DH.Domain.Adapters.Authentication;

namespace DH.Api.Extensions;

public static class Features
{
    /// <summary>
    /// Allowing RabbitMQ to be used in the application.
    /// Turned OFF until moving the statistics to separate microservice.
    /// </summary>
    public static IServiceCollection AddRabbitMQ(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IEventPublisherService, EventPublisherService>();

        var rabbitMqConfig = configuration.GetSection("RabbitMq").Get<RabbitMqOptions>()
            ?? throw new Exception("Failed to load RabbitMQ configuration. Ensure 'RabbitMq' section exists in appsettings.json.");
        // Register RabbitMqOptions as a singleton
        services.AddSingleton(rabbitMqConfig);

        services.AddScoped<IRabbitMqUserContextFactory, RabbitMqUserContextFactory>();

        services.AddScoped<IRabbitMqClient>(sp =>
        {
            // Retrieve IUserContextFactory
            var userContextFactory = sp.GetRequiredService<IUserContextFactory>();
            var userContext = userContextFactory.GetUserContextForB2b();

            // Retrieve IRabbitMqUserContextFactory
            var rabbitMqUserContextFactory = sp.GetRequiredService<IRabbitMqUserContextFactory>();

            // Transfer values without direct reference to IUserContext
            rabbitMqUserContextFactory.SetDefaultUserContext(new RabbitMqUserContext
            {
                UserId = userContext.UserId,
                IsAuthenticated = userContext.IsAuthenticated,
                RoleKey = userContext.RoleKey,
                Token = userContext.Token
            });

            return new RabbitMqClient(rabbitMqConfig.EnableMessageQueue, rabbitMqConfig.HostName, rabbitMqConfig.ExchangeName, rabbitMqUserContextFactory);
        });
        return services;
    }
}
