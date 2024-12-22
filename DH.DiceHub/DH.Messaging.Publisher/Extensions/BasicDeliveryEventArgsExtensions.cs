using System.Text;
using DH.Messaging.Publisher.Authentication;
using RabbitMQ.Client.Events;

namespace DH.Messaging.Publisher.Extensions;

public static class BasicDeliveryEventArgsExtensions
{
    public static IRabbitMqUserContext? GetUserContextFromEvent(this BasicDeliverEventArgs args)
    {
        if (args.BasicProperties.Headers != null && args.BasicProperties.Headers.ContainsKey("Authorization"))
        {
            var headerValue = args.BasicProperties.Headers["Authorization"];

            var jwtToken = headerValue is byte[] bytes
                ? Encoding.UTF8.GetString(bytes).Replace("Bearer ", "")
                : null;

            // Retrieve the UserId
            var userId = args.BasicProperties.Headers.ContainsKey("UserId") &&
                         args.BasicProperties.Headers["UserId"] is byte[] userIdBytes
                ? Encoding.UTF8.GetString(userIdBytes)
                : null;

            // Retrieve the RoleKey and parse it as an integer
            var roleKey = args.BasicProperties.Headers.ContainsKey("Role") &&
                          args.BasicProperties.Headers["Role"] is byte[] roleBytes
                ? int.TryParse(Encoding.UTF8.GetString(roleBytes), out var roleInt) ? roleInt : (int?)null
                : null;

            if (!roleKey.HasValue || userId is null || jwtToken is null)
            {
                return null;
            }

            return new RabbitMqUserContext()
            {
                RoleKey = roleKey.Value,
                UserId = userId,
                Token = jwtToken
            };
        }

        return null;
    }
}
