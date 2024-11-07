using DH.Domain.Adapters.PushNotifications.Messages.Common;
using DH.Domain.Adapters.PushNotifications.Messages.Models;

namespace DH.Domain.Adapters.PushNotifications;

public interface IPushNotificationsService
{
    Task SendMessageAsync(MessageRequest message);
    Task SendMultipleMessagesAsync(MultipleMessageRequest message);
    Task<IEnumerable<GetUserNotificationsModel>> GetNotificationsByUserId();
    Task MarkedNotificationAsViewed(int notificationId);
    Task<bool> AreAnyActiveNotifcations();
}
