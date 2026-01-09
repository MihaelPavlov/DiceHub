namespace DH.Domain.Entities;

public class TenantSetting
{
    public int Id { get; set; }

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
    public string DaysOff { get; set; } = string.Empty;

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
    public string ReservationHours { get; set; } = string.Empty;

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
}
