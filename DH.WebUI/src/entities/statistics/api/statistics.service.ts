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

@Injectable({
  providedIn: 'root',
})
export class StatisticsService {
  constructor(private readonly api: RestApiService) {
  }

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
      },{
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
      },{
        base: ApiBase.Statistics,
      }
    );
  }
}
