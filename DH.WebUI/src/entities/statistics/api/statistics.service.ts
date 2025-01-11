import { GetEventAttendanceChartData } from './../models/event-attendance-chart.model';
import { Injectable } from '@angular/core';
import {
  ApiBase,
  RestApiService,
} from '../../../shared/services/rest-api.service';
import { GetActivityChartData } from '../models/activity-chart.model';
import { Observable } from 'rxjs';
import { PATH } from '../../../shared/configs/path.config';
import { ChartActivityType } from '../enums/chart-activity-type.enum';
import { OperationResult } from '../../../shared/models/operation-result.model';
import { GetReservationChartData } from '../models/reservation-chart.model';
import { GetCollectedRewardsByDates } from '../models/collected-rewards-by-dates.model';
import { GetExpiredCollectedRewardsChart } from '../models/expired-collected-rewards-chart.model';

@Injectable({
  providedIn: 'root',
})
export class StatisticsService {
  constructor(private readonly api: RestApiService) {}

  public getActivityChartData(
    type: ChartActivityType,
    rangeStart: string,
    rangeEnd?: string
  ): Observable<OperationResult<GetActivityChartData> | null> {
    return this.api.post<OperationResult<GetActivityChartData>>(
      `/${PATH.STATISTICS.CORE}/${PATH.STATISTICS.GET_ACTIVITY_CHART_DATA}`,
      {
        type,
        rangeStart,
        rangeEnd,
      },
      {
        base: ApiBase.Statistics,
      }
    );
  }

  public getReservationChartData(
    fromDate: string,
    toDate: string
  ): Observable<OperationResult<GetReservationChartData> | null> {
    return this.api.post<OperationResult<GetReservationChartData>>(
      `/${PATH.STATISTICS.CORE}/${PATH.STATISTICS.GET_RESERVATION_CHART_DATA}`,
      {
        fromDate,
        toDate,
      },
      {
        base: ApiBase.Statistics,
      }
    );
  }

  public getEventAttendanceChartData(
    fromDate: string,
    toDate: string
  ): Observable<OperationResult<GetEventAttendanceChartData> | null> {
    return this.api.post<OperationResult<GetEventAttendanceChartData>>(
      `/${PATH.STATISTICS.CORE}/${PATH.STATISTICS.GET_EVENT_ATTENDANCE_CHART_DATA}`,
      {
        fromDate,
        toDate,
      },
      {
        base: ApiBase.Statistics,
      }
    );
  }

  public getEventAttendanceByIds(
    eventIds: number[]
  ): Observable<OperationResult<GetEventAttendanceChartData> | null> {
    return this.api.post<OperationResult<GetEventAttendanceChartData>>(
      `/${PATH.STATISTICS.CORE}/${PATH.STATISTICS.GET_EVENT_ATTENDANCE_BY_IDS}`,
      {
        eventIds,
      },
      {
        base: ApiBase.Statistics,
      }
    );
  }

  public getCollectedRewardsByDates(
    fromDate: string,
    toDate: string
  ): Observable<OperationResult<GetCollectedRewardsByDates[]> | null> {
    return this.api.post<OperationResult<GetCollectedRewardsByDates[]>>(
      `/${PATH.STATISTICS.CORE}/${PATH.STATISTICS.GET_COLLECTED_REWARDS_BY_DATES}`,
      {
        fromDate,
        toDate,
      },
      {
        base: ApiBase.Statistics,
      }
    );
  }

  public getExpiredCollectedRewardChartData(
    year: number
  ): Observable<OperationResult<GetExpiredCollectedRewardsChart> | null> {
    return this.api.post<OperationResult<GetExpiredCollectedRewardsChart>>(
      `/${PATH.STATISTICS.CORE}/${PATH.STATISTICS.GET_EXPIRED_COLLECTED_REWARDS_CHART_DATA}`,
      {
        year,
      },
      {
        base: ApiBase.Statistics,
      }
    );
  }
}
