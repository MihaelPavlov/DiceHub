import { Pipe, PipeTransform } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';

@Pipe({
  name: 'calculateRemainingDays',
  pure: true,
})
export class CalculateRemainingDaysPipe implements PipeTransform {
  constructor(private readonly translationService: TranslateService) {}

  public transform(startDate: Date): string {
    const currentDate = new Date();
    const targetDate = new Date(startDate);

    currentDate.setHours(0, 0, 0, 0);
    targetDate.setHours(0, 0, 0, 0);

    const differenceInTime = targetDate.getTime() - currentDate.getTime();
    const differenceInDays = Math.ceil(differenceInTime / (1000 * 3600 * 24));

    if (differenceInDays > 0) {
      return this.translationService.instant(
        'events.remaining_days.days_left',
        {
          count: differenceInDays,
        }
      );
    } else if (differenceInDays === 0) {
      return this.translationService.instant('events.remaining_days.today');
    } else {
      return this.translationService.instant('events.remaining_days.days_ago', {
        count: Math.abs(differenceInDays),
      });
    }
  }
}
