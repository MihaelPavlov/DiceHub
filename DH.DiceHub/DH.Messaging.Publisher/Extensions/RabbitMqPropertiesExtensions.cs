﻿using DH.Messaging.Publisher.Authentication;
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

    public static BasicProperties AddUserProps(this BasicProperties props, IRabbitMqUserContextFactory? _userContextFactory)
    {
        var userContextFactory = _userContextFactory
            ?? throw new ArgumentException("The rabbit mq User Context Factory is missing");

        var userContext = userContextFactory.CreateUserContext();

        if (string.IsNullOrEmpty(userContext.Token))
            throw new ArgumentException("Token is missing");

        props.Headers = new Dictionary<string, object?>
        {
            { "Authorization", $"Bearer {userContext.Token}" },
            { "UserId", $"{userContext.UserId}" },
            { "Role", $"{userContext.RoleKey}" }
        };

        return props;
    }

    public static BasicProperties AddUserProps(this BasicProperties props, IRabbitMqUserContext rabbitMqUserContext)
    {
        props.Headers = new Dictionary<string, object?>
        {
            { "Authorization", $"Bearer {rabbitMqUserContext.Token}" },
             { "UserId", $"{rabbitMqUserContext.UserId}" },
            { "Role", $"{rabbitMqUserContext.RoleKey}" }
        };

        return props;
    }
}
