export class DateHelper {
  static readonly DATE_FORMAT = 'yyyy-MM-dd';
  static readonly TIME_FORMAT = 'HH:mm';

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
