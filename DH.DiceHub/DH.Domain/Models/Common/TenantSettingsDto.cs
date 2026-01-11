using DH.Domain.Adapters.Localization;
using DH.Domain.Enums;
using DH.Domain.Models;
using static DH.OperationResultCore.Exceptions.ValidationErrorsException;

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
    ///Showing the next reset day to the owner
    /// </summary>
    public DateTime? NextResetTimeOfPeriod { get; set; }

    /// <summary>
    /// Which days are off for the facility
    /// </summary>
    public string[] DaysOff { get; set; } = [];

    /// <summary>
    /// Defines the start of working hours for the facility
    /// </summary>
    public string StartWorkingHours { get; set; } = string.Empty;

    /// <summary>
    /// Defines the end of working hours for the facility
    /// </summary>
    public string EndWorkingHours { get; set; } = string.Empty;

    /// <summary>
    /// Defines the number of hours to delay the initiation of the new challenge
    /// </summary>
    public int ChallengeInitiationDelayHours { get; set; }

    /// <summary>
    /// These hours should correspond to the operational hours of the facility.
    /// </summary>
    public string[] ReservationHours { get; set; } = [];

    /// <summary>
    /// Grace/Bonus time that we give to each reservation before is closed.
    /// </summary>
    public int BonusTimeAfterReservationExpiration { get; set; }

    /// <summary>
    /// Phone Number
    /// </summary>
    public string PhoneNumber { get; set; } = string.Empty;

    /// <summary>
    /// Club Name
    /// </summary>
    public string ClubName { get; set; } = string.Empty;

    /// <summary>
    /// Is Custom Period On
    /// </summary>
    public bool IsCustomPeriodOn { get; set; }

    /// <summary>
    /// Is Custom Period Setup Completed
    /// </summary>
    public bool IsCustomPeriodSetupComplete { get; set; }

    public bool FieldsAreValid(out List<ValidationError> validationErrors, ILocalizationService localizationService)
    {
        var errors = new List<ValidationError>();

        if (AverageMaxCapacity <= 0)
            errors.Add(new ValidationError(nameof(AverageMaxCapacity), localizationService["TenantSettingsAverageMaxCapacity"]));

        if (ChallengeRewardsCountForPeriod < 4 || ChallengeRewardsCountForPeriod > 8)
            errors.Add(new ValidationError(nameof(ChallengeRewardsCountForPeriod), localizationService["TenantSettingsChallengeRewardsCountForPeriod"]));

        if (!Enum.TryParse<TimePeriodType>(PeriodOfRewardReset, out var _))
            errors.Add(new ValidationError(nameof(PeriodOfRewardReset), localizationService["TenantSettingsPeriodOfRewardReset"]));

        if (!Enum.TryParse<WeekDays>(ResetDayForRewards, out var _))
            errors.Add(new ValidationError(nameof(ResetDayForRewards), localizationService["TenantSettingsResetDayForRewards"]));

        foreach (var dayOff in DaysOff)
        {
            if (!Enum.TryParse<WeekDays>(dayOff, out var _))
            {
                errors.Add(new ValidationError(nameof(DaysOff), localizationService["TenantSettingsDaysOff"]));
                break;
            }
        }

        if (string.IsNullOrEmpty(StartWorkingHours))
            errors.Add(new ValidationError(nameof(StartWorkingHours), localizationService["TenantSettingsStartWorkingHours"]));

        if (string.IsNullOrEmpty(EndWorkingHours))
            errors.Add(new ValidationError(nameof(EndWorkingHours), localizationService["TenantSettingsEndWorkingHours"]));

        if (ChallengeInitiationDelayHours <= 1 || ChallengeInitiationDelayHours > 12)
            errors.Add(new ValidationError(nameof(ChallengeInitiationDelayHours), localizationService["TenantSettingsChallengeInitiationDelayHours"]));

        if (ReservationHours.Length < 3)
            errors.Add(new ValidationError(nameof(ReservationHours), localizationService["TenantSettingsReservationHours"]));

        if (string.IsNullOrEmpty(PhoneNumber))
            errors.Add(new ValidationError(nameof(PhoneNumber), localizationService["TenantSettingsPhoneNumber"]));

        if (string.IsNullOrEmpty(ClubName))
            errors.Add(new ValidationError(nameof(ClubName), localizationService["TenantSettingsClubName"]));

        validationErrors = errors;

        return !validationErrors.Any();
    }
}
