namespace DH.Domain.Adapters.PushNotifications;

public interface IPushNotificationsService
{
    Task SendMessage(MessageRequest message);
}
