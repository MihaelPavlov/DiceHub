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
    Task SendUserNotificationAsync<TPayload>(TPayload payload)
        where TPayload : RenderableNotification;

    /// <summary>
    /// Sends a push notification to a specified list of users.
    /// </summary>
    /// <param name="userIds">The list of users with the specified role who should receive the notification.</param>
    /// <param name="message">The message details, including title and body, to be sent to the users.</param>
    /// <param name="cancellationToken">Cancellation Token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task SendNotificationToUsersAsync<TPayload>(
          List<string> userIds, TPayload payload, CancellationToken cancellationToken)
              where TPayload : RenderableNotification;

    Task<IEnumerable<GetUserNotificationsModel>> GetNotificationsByUserId(int page, int pageSize, CancellationToken cancellationToken);
    Task MarkedNotificationAsViewed(int notificationId, CancellationToken cancellationToken);
    Task ClearUserAllNotifications(CancellationToken cancellationToken);
    Task MarkedAsViewAllUserNotifications(CancellationToken cancellationToken);
    Task<bool> AreAnyActiveNotifcations(CancellationToken cancellationToken);
}
