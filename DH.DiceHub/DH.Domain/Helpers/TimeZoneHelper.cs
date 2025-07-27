using NodaTime;

namespace DH.Domain.Helpers;

public static class TimeZoneHelper
{
    public static DateTime? ConvertToUserLocalTime(this DateTime utcDateTime, string timeZoneId)
    {
        var instant = Instant.FromDateTimeUtc(utcDateTime);
        var tzdb = DateTimeZoneProviders.Tzdb;

        var userZone = tzdb.GetZoneOrNull(timeZoneId);
        if (userZone == null)
            return null;

        var zonedDateTime = instant.InZone(userZone);
        return zonedDateTime.ToDateTimeUnspecified();
    }

    public static (DateTime LocalTime, bool IsUtcFallback) GetUserLocalOrUtcTime(
         DateTime utcTime,
         string timeZoneId)
    {
        var localTime = utcTime.ConvertToUserLocalTime(timeZoneId);
        return localTime.HasValue
            ? (localTime.Value, false)
            : (utcTime, true);
    }
}