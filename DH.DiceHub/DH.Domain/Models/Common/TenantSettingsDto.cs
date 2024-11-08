using DH.Domain.Models;
using DH.Domain.Enums;
using static DH.Domain.Exceptions.ValidationErrorsException;

namespace DH.Domain.Entities;

public class TenantSettingDto : IValidableFields
{
    public int? Id { get; set; }

    /// <summary>
    /// Used to have the avarage capacity of the place
    /// </summary>
    public int AverageMaxCapacity { get; set; }

    /// <summary>
    /// The rewards that the user can receive in specific timePeriod
    /// </summary>
    public int ChallengeRewardsCountForPeriod { get; set; }

    /// <summary>
    /// Period of the rewards reset. Weekly, Monthly and etc
    /// </summary>
    public string PeriodOfRewardReset { get; set; } = string.Empty;

    /// <summary>
    /// In which day at 12:00PM the rewards are gonna be reset
    /// </summary>
    public string ResetDayForRewards { get; set; } = string.Empty;

    /// <summary>
    /// Defines the number of hours to delay the initiation of the new challenge
    /// </summary>
    public int ChallengeInitiationDelayHours { get; set; }

    public bool FieldsAreValid(out List<ValidationError> validationErrors)
    {
        var errors = new List<ValidationError>();

        if (AverageMaxCapacity <= 0)
            errors.Add(new ValidationError(nameof(AverageMaxCapacity),
                "Average Max Capacity of the place should be more then zero."));

        if (ChallengeRewardsCountForPeriod < 4 || ChallengeRewardsCountForPeriod > 8)
            errors.Add(new ValidationError(nameof(ChallengeRewardsCountForPeriod),
                $"Challenge Rewards Count For Period should be between 4 and 8 rewards."));

        if (!Enum.TryParse<TimePeriodType>(PeriodOfRewardReset, out var parsedPeriodOfRewardReset))
            errors.Add(new ValidationError(nameof(PeriodOfRewardReset),
                $"{parsedPeriodOfRewardReset} is not a valid value."));

        if (!Enum.TryParse<WeekDays>(ResetDayForRewards, out var parsedResetDayForRewards))
            errors.Add(new ValidationError(nameof(ResetDayForRewards),
                $"{parsedResetDayForRewards} is not a valid value "));

        if (ChallengeInitiationDelayHours <= 1 || ChallengeInitiationDelayHours > 12)
            errors.Add(new ValidationError(nameof(ChallengeInitiationDelayHours),
                $"Available hours should be between 2 and 12."));


        validationErrors = errors;

        return !validationErrors.Any();
    }
}