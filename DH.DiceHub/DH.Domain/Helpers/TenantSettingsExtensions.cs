namespace DH.Domain.Helpers;

public static class TenantSettingsExtensions
{
    /// <summary>
    /// Checks if today is a day off based on the DaysOff string (comma-separated day names, e.g., "Monday,Sunday").
    /// </summary>
    public static bool IsTodayDayOff(this string daysOff)
    {
        if (string.IsNullOrWhiteSpace(daysOff))
            return false;

        var daysOffList = daysOff
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(d => d.Trim())
            .ToList();

        var todayName = DateTime.UtcNow.DayOfWeek.ToString();

        return daysOffList.Contains(todayName, StringComparer.OrdinalIgnoreCase);
    }
}