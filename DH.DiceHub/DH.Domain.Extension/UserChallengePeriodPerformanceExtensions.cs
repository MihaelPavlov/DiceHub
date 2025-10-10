using DH.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace DH.Domain.Extensions;

public static class UserChallengePeriodPerformanceExtensions
{
    public static UserChallengePeriodPerformance? GetActiveUserCustomPeriod(
        this List<UserChallengePeriodPerformance> activePeriods,
        ILogger logger,
        string userId)
    {
        if (activePeriods.Count == 0)
        {
            logger.LogWarning("Active Custom Period was not found for UserId {UserId}", userId);
            return null;
        }

        if (activePeriods.Count > 1)
        {
            logger.LogWarning("Active Custom Period can't be more then 1(One). UserId {UserId}", userId);
            return null;
        }

        return activePeriods.First();
    }

    public static UserChallengePeriodPerformance? GetActiveUserPeriod(
        this List<UserChallengePeriodPerformance> activePeriods,
        ILogger logger,
        string userId)
    {
        if (activePeriods.Count == 0)
        {
            logger.LogWarning("There is no active user period performance. UserId {UserId}", userId);
            return null;
        }

        if (activePeriods.Count > 1)
        {
            logger.LogWarning("Active user period performance can't be more then 1(One). UserId {UserId}", userId);
            return null;
        }

        return activePeriods.First();
    }
}
