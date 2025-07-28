import { IScheduleJobInfo } from '../../../../entities/scheduler/models/schedule-job-info.model';
import { DateHelper } from '../../../../shared/helpers/date-helper';
import { SchedulerService } from './../../../../entities/scheduler/api/scheduler.service';
import { Component } from '@angular/core';

@Component({
  selector: 'app-jobs',
  templateUrl: 'jobs.component.html',
  styleUrl: 'jobs.component.scss',
})
export class JobsComponent {
  public readonly DATE_FORMAT: string = DateHelper.DATE_TIME_FORMAT;

  public jobs: IScheduleJobInfo[] = [];
  constructor(private readonly schedulerService: SchedulerService) {
    this.schedulerService.getScheduleJobs().subscribe({
      next: (result) => {
        if (result) {
          this.jobs = result;
        }
      },
    });
  }
}
