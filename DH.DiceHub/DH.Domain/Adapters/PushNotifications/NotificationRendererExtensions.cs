using DH.Domain.Adapters.Localization;
using DH.Domain.Helpers;
using System.Globalization;

namespace DH.Domain.Adapters.PushNotifications;

public static class NotificationRendererExtensions
{
    public static string PersonWord(this ILocalizationService localizer, int count) =>
        count == 1 ? localizer["W_Person"] : localizer["W_People"];

    public static string ToUserFormattedString(
        this DateTime utcDateTime,
        string userTimeZone,
        string userLanguage,
        ILocalizationService localizer)
    {
        var (localTime, isUtcFallback) = TimeZoneHelper.GetUserLocalOrUtcTime(utcDateTime, userTimeZone);

        var culture = new CultureInfo(userLanguage);
        var formattedTime = localTime.ToString(DateValidator.DATE_TIME_FORMAT, culture);

        if (isUtcFallback)
            formattedTime += $" {localizer["UtcFallbackNotice"]}";

        return formattedTime;
    }
}
