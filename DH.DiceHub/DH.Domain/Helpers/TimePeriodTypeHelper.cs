using DH.Domain.Enums;

namespace DH.Domain.Helpers;

public static class TimePeriodTypeHelper
{
    public static int GetDays(this TimePeriodType type)
    {
        switch (type)
        {
            case TimePeriodType.Weekly:
                return 7;
            case TimePeriodType.Monthly:
                int currentMonth = DateTime.UtcNow.Month;
                int currentYear = DateTime.UtcNow.Year;
                return DateTime.DaysInMonth(currentYear, currentMonth);
            case TimePeriodType.Yearly:
                throw new NotImplementedException("TimePeriodType for Yearly period is not implemented");
            default:
                return 7;
        }
    }

    /// <param name="timeZoneId">
    /// The club's IANA time zone (e.g. "Europe/Sofia"). Both the weekly and the
    /// monthly reset land on local midnight following the chosen day in this zone.
    /// Null/invalid falls back to <see cref="TimeZoneResolver.DefaultTimeZoneId"/>.
    /// </param>
    public static DateTime CalculateNextResetDate(TimePeriodType periodType, string resetDayForRewards, string? timeZoneId)
    {
        var tz = TimeZoneResolver.Resolve(timeZoneId);
        var nowLocal = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, tz);

        if (periodType == TimePeriodType.Weekly)
        {
            // Parse ResetDayForRewards as a DayOfWeek
            if (Enum.TryParse<DayOfWeek>(resetDayForRewards, true, out var resetDay))
            {
                int daysUntilReset = ((int)resetDay - (int)nowLocal.DayOfWeek + 7) % 7;

                // The reset lands at the end of the chosen weekday, i.e. local
                // midnight of the following day.
                var resetLocal = nowLocal.Date.AddDays(daysUntilReset + 1);
                return TimeZoneInfo.ConvertTimeToUtc(resetLocal, tz);
            }
        }
        else if (periodType == TimePeriodType.Monthly)
        {
            // Parse ResetDayForRewards as an integer representing day of the month
            if (int.TryParse(resetDayForRewards, out var resetDayOfMonth))
            {
                var year = nowLocal.Year;
                var month = nowLocal.Month;

                // If the reset day has already passed this month, roll to next month.
                if (nowLocal.Day >= resetDayOfMonth)
                {
                    var next = nowLocal.AddMonths(1);
                    year = next.Year;
                    month = next.Month;
                }

                // Clamp for short months (e.g. reset day 31 in February).
                var day = Math.Min(resetDayOfMonth, DateTime.DaysInMonth(year, month));

                // End of the chosen day => local midnight of the following day.
                var resetLocal = new DateTime(year, month, day, 0, 0, 0, DateTimeKind.Unspecified).AddDays(1);
                return TimeZoneInfo.ConvertTimeToUtc(resetLocal, tz);
            }
        }
        else if (periodType == TimePeriodType.Yearly)
        {
            throw new NotImplementedException("Functionality for Yearly period is not implemented");
        }

        // Fallback for an unparseable reset day: end of today, local time.
        return TimeZoneInfo.ConvertTimeToUtc(nowLocal.Date.AddDays(1), tz);
    }
}
