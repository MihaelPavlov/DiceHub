import { TimePeriodType } from '../../common/enum/time-period-type.enum';

export interface IUserChallengePeriodPerformance {
  id: number;
  points: number;
  startDate: Date;
  endDate: Date;
  timePeriodType: TimePeriodType;
}
