namespace DH.Messaging.Publisher;

public interface IRabbitMqClient
{
    void Setup(string exchangeName, string queueName, string routingKey);
    Task Publish<T>(string exchange, string routingKey, T message);
    Task Consume(string queueName, Func<string, Task> onMessageReceived);
}