namespace DH.Domain.Models.Common;

public class RabbitMqOptions
{
    public string HostName { get; set; } = string.Empty;
    public string ExchangeName { get; set; } = string.Empty;
    public RabbitMqQueues Queues { get; set; } = null!;
    public RabbitMqRoutingKeys RoutingKeys { get; set; } = null!;
}

public class RabbitMqQueues
{
    public string StatisticsQueue { get; set; } = string.Empty;
}

public class RabbitMqRoutingKeys
{
    public string ClubActivityDetected { get; set; } = string.Empty;
    public string EventAttendanceDetected { get; set; } = string.Empty;
    public string ReservationProcessingOutcome { get; set; } = string.Empty;
}
