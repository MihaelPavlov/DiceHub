import { DateHelper } from './../helpers/date-helper';
import { Pipe, PipeTransform } from '@angular/core';
import { DatePipe } from '@angular/common';

@Pipe({
  name: 'parseDateTag',
  standalone: false, // true if you're using standalone components
  pure: true,
})
export class ParseDateTagPipe implements PipeTransform {
  constructor(private datePipe: DatePipe) {}

  transform(value: string): string {
    if (!value) return '';

    return value.replace(
      /<datetime>(.*?)<\/datetime>/g,
      (_, dateStr: string) => {
        try {
          const date = new Date(dateStr);
          if (isNaN(date.getTime())) return dateStr;
          return this.datePipe.transform(date, DateHelper.DATE_TIME_FORMAT) ?? dateStr;
        } catch {
          return dateStr;
        }
      }
    );
  }
}
