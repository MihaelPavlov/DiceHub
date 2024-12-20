using DH.Messaging.Publisher;
using DH.ServiceBusWorker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

public class Program
{
    /// <summary>
    /// Role which is used as aabel for Azure Application Configuration and Application Insights
    /// </summary>
    public const string RoleName = "participationagreement-workerservice";

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
                // Configure RabbitMQ Client
                services.AddSingleton<IRabbitMqClient>(sp =>
                {
                    var client = new RabbitMqClient("localhost", "my_exchange");

                    client.Setup("my_exchange", "participation.agreement.queue", "participation.agreement.activated");
                    return client;
                });

                // Add RabbitMQ Worker
                services.AddScopedRabbitMqWorker<ParticipationAgreementActivatedMessage, ParticipationAgreementActivatedHandler>("participation.agreement.queue");
            });
    }
}