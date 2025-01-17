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

    public static DateTime CalculateNextResetDate(TimePeriodType periodType, string resetDayForRewards)
    {
        DateTime nextResetDate = DateTime.UtcNow;
        int resetHour = 12; // Default to 12:00 PM

        if (periodType == TimePeriodType.Weekly)
        {
            // Parse ResetDayForRewards as a DayOfWeek
            if (Enum.TryParse<DayOfWeek>(resetDayForRewards, true, out var resetDayOfWeek))
            {
                // Calculate next occurrence of specified day at 12:00 PM
                int daysUntilReset = ((int)resetDayOfWeek - (int)DateTime.UtcNow.DayOfWeek + 7) % 7;
                nextResetDate = DateTime.UtcNow.Date.AddDays(daysUntilReset).AddHours(resetHour);
            }
        }
        else if (periodType == TimePeriodType.Monthly)
        {
            // Parse ResetDayForRewards as an integer representing day of the month
            if (int.TryParse(resetDayForRewards, out var resetDayOfMonth))
            {
                int currentMonth = DateTime.UtcNow.Month;
                int currentYear = DateTime.UtcNow.Year;

                // Check if reset day has already passed this month
                if (DateTime.UtcNow.Day < resetDayOfMonth)
                {
                    nextResetDate = new DateTime(currentYear, currentMonth, resetDayOfMonth, resetHour, 0, 0, DateTimeKind.Utc);
                }
                else
                {
                    // Set to reset day of following month if day has passed
                    DateTime nextMonth = DateTime.UtcNow.AddMonths(1);
                    nextResetDate = new DateTime(nextMonth.Year, nextMonth.Month, resetDayOfMonth, resetHour, 0, 0, DateTimeKind.Utc);
                }
            }
        }
        else if (periodType == TimePeriodType.Yearly)
        {
            throw new NotImplementedException("Functionality for Yearly period is not implemented");
        }

        return nextResetDate;
    }
}
