using DH.Messaging.Publisher.Authentication;
using RabbitMQ.Client;

namespace DH.Messaging.Publisher.Extensions;

public static class RabbitMqPropertiesExtensions
{
    public static BasicProperties InitializeBasicProperties(this BasicProperties props)
    {
        props.ContentType = "text/plain";
        props.DeliveryMode = DeliveryModes.Persistent;
        props.Expiration = "36000000";

        return props;
    }

    public static BasicProperties AddUserToken(this BasicProperties props, IRabbitMqUserContextFactory? _userContextFactory)
    {
        var userContextFactory = _userContextFactory
            ?? throw new ArgumentException("The rabbit mq User Context Factory is missing");

        var token = userContextFactory.CreateUserContext().Token;

        if (string.IsNullOrEmpty(token))
            throw new ArgumentException("Token is missing");

        props.Headers = new Dictionary<string, object?>
        {
            { "Authorization", $"Bearer {token}" }
        };

        return props;
    }

    public static BasicProperties AddUserToken(this BasicProperties props, string token)
    {
        props.Headers = new Dictionary<string, object?>
        {
            { "Authorization", $"Bearer {token}" }
        };

        return props;
    }
}
