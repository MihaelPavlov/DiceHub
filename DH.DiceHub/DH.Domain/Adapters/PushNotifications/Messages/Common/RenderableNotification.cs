using DH.Domain.Adapters.Localization;
using DH.Domain.Adapters.PushNotifications.Messages.Models;

namespace DH.Domain.Adapters.PushNotifications.Messages.Common;

/// <summary>
/// Placeholder class for notification
/// </summary>
public abstract class RenderableNotification
{
    public string DeviceToken { get; set; } = string.Empty;

    //Change it to this later. All derived classes MUST implement this
    //public abstract NotificationPayload Render(
    //    ILocalizationService localizer,
    //    string userTimeZone,
    //    string userLanguage);

    // Optional: allow payloads to render themselves
    public virtual NotificationPayload? Render(
        ILocalizationService localizer,
        string userTimeZone,
        string userLanguage)
    {
        return null;
    }
}
