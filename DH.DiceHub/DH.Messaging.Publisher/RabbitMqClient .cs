using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace DH.Messaging.Publisher;

public class RabbitMqClient : IRabbitMqClient
{
    private readonly IConnection _connection;
    private readonly IChannel _channel;

    public RabbitMqClient(string hostName, string exchangeName)
    {
        var factory = new ConnectionFactory() { HostName = hostName };

        _connection = factory.CreateConnectionAsync(CancellationToken.None).Result;
        _channel = _connection.CreateChannelAsync().Result;

        // Declare the exchange
        _channel.ExchangeDeclareAsync(exchange: exchangeName, type: ExchangeType.Topic);
    }

    public void Setup(string exchangeName, string queueName, string routingKey)
    {
        // Declare the exchange
        _channel.ExchangeDeclareAsync(exchange: exchangeName, type: ExchangeType.Topic).GetAwaiter().GetResult(); ;

        // Declare the queue
        _channel.QueueDeclareAsync(queue: queueName, durable: true, exclusive: false, autoDelete: false, arguments: null).GetAwaiter().GetResult(); ;

        // Bind the queue to the exchange with the routing key
        _channel.QueueBindAsync(queue: queueName, exchange: exchangeName, routingKey: routingKey).GetAwaiter().GetResult(); ;
    }


    public async Task Publish<T>(string exchange, string routingKey, T message)
    {
        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));
        var props = new BasicProperties();
        props.ContentType = "text/plain";
        props.DeliveryMode = DeliveryModes.Persistent;
        props.Expiration = "36000000";

        await _channel.BasicPublishAsync(exchange, routingKey, false, props, body);
    }

    public async Task Consume(string queueName, Func<string, Task> onMessageReceived)
    {
        await _channel.QueueDeclareAsync(queue: queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);

        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += async (model, eventArgs) =>
        {
            var body = eventArgs.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);

            await onMessageReceived(message);
        };

        await _channel.BasicConsumeAsync(queue: queueName, autoAck: true, consumer: consumer);
    }

    public async Task Dispose()
    {
        await _channel.CloseAsync();
        await _connection.CloseAsync();
    }
}