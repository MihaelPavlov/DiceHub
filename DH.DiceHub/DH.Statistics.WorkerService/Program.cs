using DH.Messaging.HttpClient;
using DH.Messaging.Publisher;
using DH.ServiceBusWorker;
using DH.Statistics.WorkerService;
using DH.Statistics.WorkerService.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Configuration;

// Run the Docker RABBITMQ -> docker run -it --rm --name mymq -p 5672:5672 -p 15672:15672 rabbitmq:3-management
public class Program
{
    protected Program() { }

    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args)
    {
        return Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostBuilderContext, services) =>
            {
                // Bind RabbitMQ settings from appsettings.json
                var rabbitMqConfig = hostBuilderContext.Configuration.GetSection("RabbitMq").Get<RabbitMqOptions>()
                    ?? throw new ConfigurationErrorsException("Failed to load RabbitMQ configuration. Ensure 'RabbitMq' section exists in appsettings.json.");

                if (string.IsNullOrEmpty(rabbitMqConfig.HostName))
                    throw new ConfigurationErrorsException("RabbitMQ HostName is missing in the configuration.");

                if (string.IsNullOrEmpty(rabbitMqConfig.ExchangeName))
                    throw new ConfigurationErrorsException("RabbitMQ ExchangeName is missing in the configuration.");

                if (string.IsNullOrEmpty(rabbitMqConfig.Queues?.ParticipationAgreementQueue))
                    throw new ConfigurationErrorsException("RabbitMQ ParticipationAgreementQueue is missing in the configuration.");

                var missingRoutingKeys = new List<string>();
                if (string.IsNullOrEmpty(rabbitMqConfig.RoutingKeys?.ParticipationAgreementActivated))
                    missingRoutingKeys.Add(nameof(rabbitMqConfig.RoutingKeys.ParticipationAgreementActivated));
                if (string.IsNullOrEmpty(rabbitMqConfig.RoutingKeys?.PartProcess))
                    missingRoutingKeys.Add(nameof(rabbitMqConfig.RoutingKeys.PartProcess));

                if (missingRoutingKeys.Any())
                    throw new ConfigurationErrorsException($"RabbitMQ RoutingKeys missing in the configuration: {string.Join(", ", missingRoutingKeys)}.");

                // Configure RabbitMQ Client
                services.AddSingleton<IRabbitMqClient>(sp =>
                {
                    var client = new RabbitMqClient(rabbitMqConfig.HostName, rabbitMqConfig.ExchangeName);

                    // Set up queues and bindings with specific routing keys
                    client.Setup(
                        rabbitMqConfig.ExchangeName,
                        rabbitMqConfig.Queues.ParticipationAgreementQueue,
                        rabbitMqConfig.RoutingKeys!.ParticipationAgreementActivated);

                    client.Setup(
                        rabbitMqConfig.ExchangeName,
                        rabbitMqConfig.Queues.ParticipationAgreementQueue,
                        rabbitMqConfig.RoutingKeys.PartProcess);

                    return client;
                });

                // Add RabbitMQ Workers
                services.AddScopedRabbitMqWorker<ParticipationAgreementActivatedMessage, ParticipationAgreementActivatedHandler>(
                    rabbitMqConfig.Queues.ParticipationAgreementQueue);

                services.AddScopedRabbitMqWorker<PartProcessMessage, PartProcessHandler>(
                    rabbitMqConfig.Queues.ParticipationAgreementQueue);

                services.AddCommunicationService();
            });
    }
}



