export interface IScheduleJobInfo {
  jobKeyName: string;
  triggerKeyName: string;
  nextFireTime: Date | null;
  previousFireTime: Date | null;
}
