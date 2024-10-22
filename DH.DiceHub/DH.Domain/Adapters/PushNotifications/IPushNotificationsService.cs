using DH.Domain.Adapters.PushNotifications.Messages.Common;

namespace DH.Domain.Adapters.PushNotifications;

public interface IPushNotificationsService
{
    Task SendMessageAsync(MessageRequest message);
    Task SendMultipleMessagesAsync(MultipleMessageRequest message);
}
