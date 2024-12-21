using System.Text;
using RabbitMQ.Client.Events;

namespace DH.Messaging.Publisher.Extensions;

public static class BasicDeliveryEventArgsExtensions
{
    public static string? GetToken(this BasicDeliverEventArgs args)
    {
        if (args.BasicProperties.Headers != null && args.BasicProperties.Headers.ContainsKey("Authorization"))
        {
            var headerValue = args.BasicProperties.Headers["Authorization"];

            var jwtToken = headerValue is byte[] bytes
                ? Encoding.UTF8.GetString(bytes).Replace("Bearer ", "")
                : null;

            return jwtToken;
        }

        return null;
    }
}
