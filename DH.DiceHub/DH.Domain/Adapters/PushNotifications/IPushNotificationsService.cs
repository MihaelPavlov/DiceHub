using DH.Domain.Adapters.Authentication.Models;
using DH.Domain.Adapters.PushNotifications.Messages.Common;
using DH.Domain.Adapters.PushNotifications.Messages.Models;

namespace DH.Domain.Adapters.PushNotifications;

public interface IPushNotificationsService
{
    /// <summary>
    /// Sends a push notification to the current logged-in user based on their device token.
    /// </summary>
    /// <param name="message">The message details including title, body, and device token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task SendUserNotificationAsync(MessageRequest message);

    /// <summary>
    /// Sends a push notification to multiple specified device tokens.
    /// </summary>
    /// <param name="message">The message details including title, body, and list of device tokens.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task SendBulkNotificationsAsync(MultipleMessageRequest message);

    /// <summary>
    /// Sends a push notification to a specified list of users.
    /// </summary>
    /// <param name="userIds">The list of users with the specified role who should receive the notification.</param>
    /// <param name="message">The message details, including title and body, to be sent to the users.</param>
    /// <param name="cancellationToken">Cancellation Token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task SendNotificationToUsersAsync(List<string> userIds, MessageRequest message, CancellationToken cancellationToken);
    Task<IEnumerable<GetUserNotificationsModel>> GetNotificationsByUserId(CancellationToken cancellationToken);
    Task MarkedNotificationAsViewed(int notificationId, CancellationToken cancellationToken);
    Task ClearUserAllNotifications(CancellationToken cancellationToken);
    Task MarkedAsViewAllUserNotifications(CancellationToken cancellationToken);
    Task<bool> AreAnyActiveNotifcations(CancellationToken cancellationToken);
}
