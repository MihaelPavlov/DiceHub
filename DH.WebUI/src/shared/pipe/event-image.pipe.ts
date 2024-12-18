import { Pipe, PipeTransform } from '@angular/core';
import { environment } from '../environments/environment.development';

@Pipe({
  name: 'eventImage',
  pure: true,
})
export class EventImagePipe implements PipeTransform {
  transform(imageId: number): string {
    const URL: string = `${environment.appUrl}/events/get-image/`;
    return `${URL}${imageId}`;
  }
}
