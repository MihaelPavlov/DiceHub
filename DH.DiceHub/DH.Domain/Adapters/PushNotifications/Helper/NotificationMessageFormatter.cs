namespace DH.Domain.Adapters.PushNotifications.Helper;

public static class NotificationMessageFormatter
{
    public static string WrapDateTime(this DateTime utcDateTime)
    {
        return $"<datetime>{utcDateTime:O}</datetime>";
    }
}
