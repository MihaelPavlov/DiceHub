import { GetEventAttendanceChartData } from './../models/event-attendance-chart.model';
import { Injectable } from '@angular/core';
import { RestApiService } from '../../../shared/services/rest-api.service';
import { GetActivityChartData } from '../models/activity-chart.model';
import { Observable } from 'rxjs';
import { PATH } from '../../../shared/configs/path.config';
import { ChartActivityType } from '../enums/chart-activity-type.enum';
import { OperationResult } from '../../../shared/models/operation-result.model';
import { GetReservationChartData } from '../models/reservation-chart.model';
import { GetCollectedRewardsByDates } from '../models/collected-rewards-by-dates.model';
import { GetExpiredCollectedRewardsChart } from '../models/expired-collected-rewards-chart.model';
import { IChallengeLeaderboard } from '../models/challenge-leaderboard.model';
import { ChallengeLeaderboardType } from '../enums/challenge-leaderboard-type.enum';
import { GamesActivityType } from '../enums/games-activity-type.enum';
import { GetGameActivityChartData } from '../models/game-activity-chart.model';
import { GetUsersWhoPlayedGameData } from '../models/game-user-activity.model';

@Injectable({
  providedIn: 'root',
})
export class StatisticsService {
  constructor(private readonly api: RestApiService) {}

  public getActivityChartData(
    type: ChartActivityType,
    rangeStart: Date,
    rangeEnd?: Date | null
  ): Observable<OperationResult<GetActivityChartData> | null> {
    return this.api.post<OperationResult<GetActivityChartData>>(
      `/${PATH.STATISTICS.CORE}/${PATH.STATISTICS.GET_ACTIVITY_CHART_DATA}`,
      {
        type,
        rangeStart,
        rangeEnd,
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
      }
    );
  }

  public getChallengeLeaderboard(
    type: ChallengeLeaderboardType
  ): Observable<OperationResult<IChallengeLeaderboard[]> | null> {
    return this.api.post<OperationResult<IChallengeLeaderboard[]>>(
      `/${PATH.STATISTICS.CORE}/${PATH.STATISTICS.GET_CHALLENGE_HISTORY_LOG}`,
      {
        type,
      }
    );
  }

  public getGameActivityChartData(
    type: GamesActivityType,
    rangeStart?: Date | null,
    rangeEnd?: Date | null
  ): Observable<OperationResult<GetGameActivityChartData> | null> {
    return this.api.post<OperationResult<GetGameActivityChartData>>(
      `/${PATH.STATISTICS.CORE}/${PATH.STATISTICS.GET_GAME_ENGAGEMENT_CHART_DATA}`,
      {
        type,
        rangeStart,
        rangeEnd,
      }
    );
  }

  public getGameUserActivityChartData(
    gameId: number,
    type: GamesActivityType,
    rangeStart?: Date | null,
    rangeEnd?: Date | null
  ): Observable<OperationResult<GetUsersWhoPlayedGameData> | null> {
    return this.api.post<OperationResult<GetUsersWhoPlayedGameData>>(
      `/${PATH.STATISTICS.CORE}/${PATH.STATISTICS.GET_GAME_USER_ENGAGEMENT_CHART_DATA}`,
      {
        gameId,
        type,
        rangeStart,
        rangeEnd,
      }
    );
  }
}
