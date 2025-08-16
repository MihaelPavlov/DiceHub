using DH.Domain.Adapters.PushNotifications.Messages.Common;
using DH.Domain.Adapters.PushNotifications.Messages.Models;

namespace DH.Domain.Adapters.PushNotifications;

public interface INotificationRenderer
{
    Task<NotificationPayload?> RenderMessageBody<TPayload>(TPayload payload, string userId) where TPayload : RenderableNotification;
}
