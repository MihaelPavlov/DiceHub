using DH.Domain.Adapters.PushNotifications.Messages.Common;
using DH.Domain.Adapters.PushNotifications.Messages.Models;

namespace DH.Domain.Adapters.PushNotifications;

public interface IPushNotificationsService
{
    Task SendMessageAsync(MessageRequest message);
    Task SendMultipleMessagesAsync(MultipleMessageRequest message);
    Task<IEnumerable<GetUserNotificationsModel>> GetNotificationsByUserId(CancellationToken cancellationToken);
    Task MarkedNotificationAsViewed(int notificationId, CancellationToken cancellationToken);
    Task<bool> AreAnyActiveNotifcations(CancellationToken cancellationToken);
}
