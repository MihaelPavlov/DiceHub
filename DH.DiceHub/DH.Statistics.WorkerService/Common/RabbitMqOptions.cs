namespace DH.Statistics.WorkerService.Common;

public class RabbitMqOptions
{
    public string HostName { get; set; } = string.Empty;
    public string ExchangeName { get; set; } = string.Empty;
    public RabbitMqQueues Queues { get; set; } = null!;
    public RabbitMqRoutingKeys RoutingKeys { get; set; } = null!;
}