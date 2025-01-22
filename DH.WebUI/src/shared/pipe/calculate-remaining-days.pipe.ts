import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'calculateRemainingDays',
  pure: true,
})
export class CalculateRemainingDaysPipe implements PipeTransform {
  transform(startDate: Date): string {
    const currentDate = new Date();
    const targetDate = new Date(startDate);

    currentDate.setHours(0, 0, 0, 0);
    targetDate.setHours(0, 0, 0, 0);

    const differenceInTime = targetDate.getTime() - currentDate.getTime();
    const differenceInDays = Math.ceil(differenceInTime / (1000 * 3600 * 24));

    if (differenceInDays > 0) {
      return `${differenceInDays}d left`;
    } else if (differenceInDays === 0) {
      return 'Today';
    } else {
      return `${Math.abs(differenceInDays)}d ago`;
    }
  }
}
