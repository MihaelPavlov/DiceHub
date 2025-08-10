import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { PATH } from '../../../shared/configs/path.config';
import { RestApiService } from '../../../shared/services/rest-api.service';
import { IScheduleJobInfo } from '../models/schedule-job-info.model';

@Injectable({
  providedIn: 'root',
})
export class SchedulerService {
  constructor(private readonly api: RestApiService) {}

  public getScheduleJobs(): Observable<IScheduleJobInfo[] | null> {
    return this.api.get<IScheduleJobInfo[]>(
      `/${PATH.SCHEDULER.CORE}/${PATH.SCHEDULER.GET_SCHEDULE_JOBS}`
    );
  }

   public runJobsForPeriod(): Observable<null> {
    return this.api.get(
      `/${PATH.SCHEDULER.CORE}/run-concurrent`
    );
  }
}
