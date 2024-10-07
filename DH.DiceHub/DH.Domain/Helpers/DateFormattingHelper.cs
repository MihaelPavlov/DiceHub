using System.Globalization;
using System.Text.RegularExpressions;

namespace DH.Domain.Helpers;

public static class DateFormattingHelper
{
    public static readonly Regex SortableDateTimeRegex = new(@"^(\d{4})-(\d\d)-(\d\d)T(\d\d):(\d\d):(\d\d)$");
    public static readonly Regex SqlDefaultDateTimeRegex = new(@"^(Jan|Feb|Mar|Apr|May|Jun|Jul|Aug|Sep|Oct|Nov|Dec)\s+(\d{1,2}) (\d{4}) (\d\d):(\d\d)(AM|PM)$");
    public const string DEFAULT_DATETIME_FORMAT = "yyyy-MM-ddTHH:mm:ss";
    public const string DATETIME_FORMAT_FOR_DISPLAY = "dd MMM yyyy";

    public static string FormatDate(string isoDateString)
    {
        if (!DateTime.TryParse(isoDateString, CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
        {
            return string.Empty;
        }

        return date.ToString(DATETIME_FORMAT_FOR_DISPLAY, CultureInfo.InvariantCulture);
    }

    public static string GetFormatedDuration(DateTime startTime, DateTime endTime)
    {
        var ts = endTime - startTime;
        return $"{ts.Hours:00}:{ts.Minutes:00}:{ts.Seconds:00}.{(ts.Milliseconds / 10):00}";
    }

    public static bool TryParseAndFormatDate(string value, out string result)
    {
        result = string.Empty;
        DateTime date;

        if (SortableDateTimeRegex.Match(value).Success)
        {
            if (!DateTime.TryParseExact(value, DEFAULT_DATETIME_FORMAT, CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
                return false;
        }
        else if (SqlDefaultDateTimeRegex.Match(value).Success)
        {
            if (!DateTime.TryParseExact(value, "MMM dd yyyy h:mmtt", CultureInfo.InvariantCulture, DateTimeStyles.None, out date)
                && !DateTime.TryParseExact(value, "MMM  d yyyy h:mmtt", CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
                return false;
        }
        else
        {
            return false;
        }

        result = date.ToString(DATETIME_FORMAT_FOR_DISPLAY, CultureInfo.InvariantCulture);
        return true;
    }
}
