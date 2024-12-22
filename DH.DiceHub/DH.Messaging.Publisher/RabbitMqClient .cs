using DH.Messaging.Publisher.Authentication;
using DH.Messaging.Publisher.Extensions;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace DH.Messaging.Publisher;

/// <summary>
/// RabbitMQ client implementation for publishing and consuming messages.
/// </summary>
public class RabbitMqClient : IRabbitMqClient
{
    private readonly IConnection _connection;
    private readonly IChannel _channel;

    /// <summary>
    /// Factory for creating user context for RabbitMQ.
    /// Which can be null when it's on the consumer side.
    /// </summary>
    private readonly IRabbitMqUserContextFactory? _rabbitMqUserContextFactory;

    /// <summary>
    /// Rabbit Mq UserContext that the publisher can use if it's want to send event from the consumer.
    /// </summary>
    private IRabbitMqUserContext? _rabbitMqUserContext;

    /// <summary>
    /// Initializes a new instance of the <see cref="RabbitMqClient"/> class.
    /// </summary>
    /// <param name="hostName">RabbitMQ server hostname.</param>
    /// <param name="exchangeName">Default exchange name for RabbitMQ.</param>
    public RabbitMqClient(string hostName, string exchangeName, IRabbitMqUserContextFactory? rabbitMqUserContextFactory = null)
    {
        _rabbitMqUserContextFactory = rabbitMqUserContextFactory;

        var factory = new ConnectionFactory() { HostName = hostName };

        _connection = factory.CreateConnectionAsync(CancellationToken.None).Result;
        _channel = _connection.CreateChannelAsync().Result;

        // Declare the exchange
        _channel.ExchangeDeclareAsync(exchange: exchangeName, type: ExchangeType.Topic);
    }

    /// <inheritdoc/>
    public void Setup(string exchangeName, string queueName, string routingKey)
    {
        // Declare the exchange
        _channel.ExchangeDeclareAsync(exchange: exchangeName, type: ExchangeType.Topic).GetAwaiter().GetResult();

        // Declare the queue
        _channel.QueueDeclareAsync(queue: queueName, durable: true, exclusive: false, autoDelete: false, arguments: null).GetAwaiter().GetResult();

        // Bind the queue to the exchange with the routing key
        _channel.QueueBindAsync(queue: queueName, exchange: exchangeName, routingKey: routingKey).GetAwaiter().GetResult();
    }

    /// <inheritdoc/>
    public async Task Publish<T>(string exchange, string routingKey, T message)
    {
        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));
        var props = new BasicProperties().InitializeBasicProperties();

        if (_rabbitMqUserContextFactory == null && this._rabbitMqUserContext != null)
        {
            props.AddUserProps(this._rabbitMqUserContext);
        }
        else
        {
            props.AddUserProps(this._rabbitMqUserContextFactory);
        }

        await _channel.BasicPublishAsync(exchange, routingKey, false, props, body);
    }

    /// <inheritdoc/>
    public async Task Consume(string queueName, Func<string, Task> onMessageReceived)
    {
        await _channel.QueueDeclareAsync(queue: queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);

        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += async (model, eventArgs) =>
        {
            var body = eventArgs.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);

            if (eventArgs.GetUserContextFromEvent() is IRabbitMqUserContext rabbitMqContext)
            {
                SetupRabbitMqUserContext(rabbitMqContext);
                await onMessageReceived(message);
            }
        };

        await _channel.BasicConsumeAsync(queue: queueName, autoAck: true, consumer: consumer);
    }

    /// <summary>
    /// Disposes the RabbitMQ client resources.
    /// </summary>
    public async Task Dispose()
    {
        await _channel.CloseAsync();
        await _connection.CloseAsync();
    }

    /// <summary>
    /// Sets the client token to be used in consuming messages.
    /// </summary>
    /// <param name="token">The client token.</param>
    private void SetupRabbitMqUserContext(IRabbitMqUserContext rabbitMqUserContext)
    {
        _rabbitMqUserContext = rabbitMqUserContext;
    }

    public IRabbitMqUserContext GetSender()
    {
        if (_rabbitMqUserContextFactory == null && this._rabbitMqUserContext != null)
            return this._rabbitMqUserContext;

        throw new ArgumentException("The rabbit mq User Context is missing");
    }
}