namespace DH.Messaging.Publisher;

/// <summary>
/// Interface for RabbitMQ client operations.
/// </summary>
public interface IRabbitMqClient
{
    /// <summary>
    /// Sets up the exchange, queue, and routing key for RabbitMQ.
    /// </summary>
    /// <param name="exchangeName">Name of the exchange.</param>
    /// <param name="queueName">Name of the queue.</param>
    /// <param name="routingKey">Routing key for the messages.</param>
    void Setup(string exchangeName, string queueName, string routingKey);

    /// <summary>
    /// Publishes a message to a RabbitMQ exchange with a specific routing key.
    /// </summary>
    /// <typeparam name="T">Type of the message to be published.</typeparam>
    /// <param name="exchange">Name of the exchange.</param>
    /// <param name="routingKey">Routing key for the message.</param>
    /// <param name="message">Message to be published.</param>
    Task Publish<T>(string exchange, string routingKey, T message);

    /// <summary>
    /// Consumes messages from a specified RabbitMQ queue.
    /// </summary>
    /// <param name="queueName">Name of the queue to consume messages from.</param>
    /// <param name="onMessageReceived">Callback to handle received messages.</param>
    Task Consume(string queueName, Func<string, Task> onMessageReceived);
}