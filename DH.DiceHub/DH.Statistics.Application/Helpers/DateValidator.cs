using MediatR;

namespace DH.Statistics.Application.Helpers;

public static class DateValidator
{
    /// <summary>
    /// Validates and parses the FromDate and ToDate properties of an IRequest object.
    /// </summary>
    /// <param name="query">The IRequest object containing FromDate and ToDate properties.</param>
    /// <returns>
    /// A tuple containing the parsed FromDate, ToDate, and an error message if any.
    /// The FromDate and ToDate will be null if parsing fails, and the ErrorMessage will contain the error details.
    /// </returns>
    public static (DateTime? FromDate, DateTime? ToDate, string? ErrorMessage) ValidateAndParseDates<T>(this IRequest<T> query)
    {
        var fromDateProperty = query.GetType().GetProperty("FromDate");
        var toDateProperty = query.GetType().GetProperty("ToDate");

        if (fromDateProperty == null || toDateProperty == null)
            return (null, null, "FromDate or ToDate property is missing");

        var fromDateValue = fromDateProperty.GetValue(query)?.ToString();
        var toDateValue = toDateProperty.GetValue(query)?.ToString();

        if (!DateTime.TryParse(fromDateValue, out var fromDateUtc))
            return (null, null, "From Date is Missing or Incorrect");

        if (!DateTime.TryParse(toDateValue, out var toDateUtc))
            return (null, null, "To Date is Missing or Incorrect");

        return (fromDateUtc, toDateUtc, null);
    }
}
