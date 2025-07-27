namespace DH.Domain.Helpers;

public static class DateValidator
{
    public const string DATE_TIME_FORMAT = "dd/MMM/yyyy HH:mm";

    /// <summary>
    /// Validates and parses the FromDate and ToDate properties of an IRequest object.
    /// </summary>
    /// <param name="query">The IRequest object containing FromDate and ToDate properties.</param>
    /// <returns>
    /// A tuple containing the parsed FromDate, ToDate, and an error message if any.
    /// The FromDate and ToDate will be null if parsing fails, and the ErrorMessage will contain the error details.
    /// </returns>
    public static (DateTime? FromDate, DateTime? ToDate, string? ErrorMessage) ValidateAndParseDates(string? fromDate, string? toDate)
    {
        if (fromDate == null || toDate == null)
            return (null, null, "FromDate or ToDate property is missing");

        if (!DateTime.TryParse(fromDate, out var fromDateUtc))
            return (null, null, "From Date is Missing or Incorrect");

        if (!DateTime.TryParse(toDate, out var toDateUtc))
            return (null, null, "To Date is Missing or Incorrect");

        fromDateUtc = fromDateUtc.ToUniversalTime();
        toDateUtc = toDateUtc.ToUniversalTime();

        return (fromDateUtc, toDateUtc, null);
    }

    public static DateTime StartOfWeek(this DateTime dt, DayOfWeek startOfWeek)
    {
        int diff = (7 + (dt.DayOfWeek - startOfWeek)) % 7;
        return dt.AddDays(-1 * diff).Date;
    }
}
