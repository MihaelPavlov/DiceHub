import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'calculateRemainingDays',
  pure: true,
})
export class CalculateRemainingDaysPipe implements PipeTransform {
  transform(startDate: Date): string {
    const currentDate = new Date();
    const startDateSubject = new Date(startDate.toString());
    const remainingDays = Math.ceil(
      (startDateSubject.getTime() - currentDate.getTime()) /
        (1000 * 60 * 60 * 24)
    );
    return `${Math.abs(remainingDays)}d`;
  }
}
