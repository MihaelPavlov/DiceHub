using DH.Messaging.HttpClient;
using DH.Messaging.HttpClient.Enums;
using DH.Messaging.Publisher;

namespace DH.Statistics.WorkerService;

public class ParticipationAgreementActivatedHandler(IAuthorizedClientFactory authorizedClientFactory, IRabbitMqClient client) : IServiceBusHandler<ParticipationAgreementActivatedMessage>
{
    readonly IAuthorizedClientFactory _authorizedClientFactory = authorizedClientFactory;
    readonly IRabbitMqClient _client = client;

    public async Task HandleMessageAsync(EventMessage<ParticipationAgreementActivatedMessage> message, string messageId, CancellationToken cancellationToken)
    {
        Console.WriteLine($"Processing message: ");

        var sender = _client.GetSender();
        var res = await _authorizedClientFactory
           .CreateClient(ApplicationApi.Statistics, sender.UserId, sender.Token)
           .BuildGet("WeatherForecast")
           .WithImpersonated()
           .SendWithResulAsync<IEnumerable<WeatherForecast>>(cancellationToken);

        //await client.Publish("exchange", "routingKey", new ParticipationAgreementActivatedMessage());
    }
}

public class WeatherForecast
{
    public DateOnly Date { get; set; }

    public int TemperatureC { get; set; }

    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

    public string? Summary { get; set; }
}