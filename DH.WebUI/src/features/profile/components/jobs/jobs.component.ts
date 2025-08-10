import { MessagingService } from './../../../../entities/messaging/api/messaging.service';
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
  public iphoneVersion: number = 0;
  public nativePlatform: string = '';
  public jobs: IScheduleJobInfo[] = [];
  constructor(
    private readonly schedulerService: SchedulerService,
    private readonly messagingService: MessagingService
  ) {
    this.iphoneVersion = this.messagingService.getIOSVersion();
    this.nativePlatform = this.messagingService.getNativePlatform();
    this.schedulerService.getScheduleJobs().subscribe({
      next: (result) => {
        if (result) {
          this.jobs = result;
        }
      },
    });
  }

  public executePeriodJobs(): void {
    this.schedulerService.runJobsForPeriod().subscribe();
  }
}
