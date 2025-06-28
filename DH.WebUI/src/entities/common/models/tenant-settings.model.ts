import { WeekDay } from '@angular/common';
import { TimePeriodType } from '../enum/time-period-type.enum';

export interface ITenantSettings {
  id?: number | null;

  /// <summary>
  /// Used to have the average capacity of the place
  /// </summary>
  averageMaxCapacity: number;

  /// <summary>
  /// The rewards that the user can receive in specific timePeriod
  /// </summary>
  challengeRewardsCountForPeriod: number;

  /// <summary>
  /// Period of the rewards reset. Weekly, Monthly and etc
  /// </summary>
  periodOfRewardReset: TimePeriodType;

  /// <summary>
  /// In which day at 12:00PM the rewards are gonna be reset
  /// </summary>
  resetDayForRewards: WeekDay;

  /// <summary>
  /// Defines the number of hours to delay the initiation of the new challenge
  /// </summary>
  challengeInitiationDelayHours: number;

  /// <summary>
  /// These hours should correspond to the operational hours of the facility.
  /// </summary>
  reservationHours: string[];

  /// <summary>
  /// Grace/Bonus time that we give to each reservation before is closed.
  /// </summary>
  bonusTimeAfterReservationExpiration: number;

  /// <summary>
  /// Phone Number
  /// </summary>
  phoneNumber: string;

  /// <summary>
  /// Club Name
  /// </summary>
  clubName: string;

  /// <summary>
  /// Is Custom Period On
  /// </summary>
  isCustomPeriodOn: boolean;
}
