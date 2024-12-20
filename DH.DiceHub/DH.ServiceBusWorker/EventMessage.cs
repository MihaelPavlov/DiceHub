namespace DH.ServiceBusWorker;

/// <summary>
/// Implementation for Event Message with custom body
/// </summary>
/// <typeparam name="T"></typeparam>
public record EventMessage<T> : EventMessage
{
    public T? Body { get; init; }
}

/// <summary>
/// Properties for event messages
/// </summary>
public record EventMessage
{

    /// <summary>
    /// The Id of the resource being sent through the message bus (if applicable)
    /// </summary>
    public string? ResourceId { get; init; }

    /// <summary>
    /// A unique Id for this publish event (deliver once cannot be guaranteed)
    /// </summary>
    public string? MessageId { get; init; }

    /// <summary>
    /// A correlationId identify context for the message
    /// </summary>
    public string? CorrelationId { get; init; }

    /// <summary>
    /// A label that can be used to add context to the message and or be used for filtering
    /// </summary>
    public string? Label { get; init; }


    /// <summary>
    /// Original Message Topic Name (useful when forwarding messages to other topics)
    /// </summary>
    public string? OriginalTopic { get; set; }


    /// <summary>
    /// The UTC time of the actual event occurrence 
    /// </summary>
    public DateTimeOffset? DateTime { get; set; }
}
