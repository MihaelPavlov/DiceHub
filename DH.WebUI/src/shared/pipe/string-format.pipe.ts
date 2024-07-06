import { Pipe, PipeTransform } from '@angular/core';

@Pipe({ name: 'stringFormat' })
export class StringFormatPipe implements PipeTransform {
  transform(value: string, args: { [key: string]: any }): string {
    if (!value) return '';

    // Replace placeholders in the format string with provided arguments
    return value.replace(/{([^}]+)}/g, (match, key) => {
      return typeof args[key] !== 'undefined' ? args[key] : match;
    });
  }
}
