export class DateHelper {
  static readonly DATE_FORMAT_FOR_INPUT = 'yyyy-MM-dd';
  static readonly DATE_FORMAT = 'dd/MMM/yyyy';
  static readonly TIME_FORMAT = 'HH:mm';
  static readonly DATE_TIME_FORMAT = 'dd/MMM/yyyy HH:mm';

  /**
   * Combines a date and time string into an ISO format date-time string.
   * @param date - The date string in 'yyyy-MM-dd' format.
   * @param time - The time string in 'HH:mm' format.
   * @returns A string representing the combined date-time in ISO format.
   */
  static combineDateAndTime(date: string, time: string): string {
    const dateTimeString = `${date}T${time}:00`;
    const parsedDate = new Date(dateTimeString).toISOString();
    return parsedDate;
  }
}
