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
    /// Period of the rewards reset. Weekyl, Monthly and etc
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
}
