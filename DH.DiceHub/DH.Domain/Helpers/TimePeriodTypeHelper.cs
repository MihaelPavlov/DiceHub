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
                int currentMonth = DateTime.Now.Month;
                int currentYear = DateTime.Now.Year;
                return DateTime.DaysInMonth(currentYear, currentMonth);
            case TimePeriodType.Yearly:
                throw new NotImplementedException("TimePeriodType for Yearly period is not implemented");
            default:
                return 7;
        }
    }
}
