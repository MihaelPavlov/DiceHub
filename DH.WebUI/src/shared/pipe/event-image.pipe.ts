import { Pipe, PipeTransform } from "@angular/core";

@Pipe({
  name: 'eventImage',
  pure: true,
})
export class EventImagePipe implements PipeTransform {
  transform(imageId: number): string {
    const baseUrl = 'https://localhost:7024/events/get-image/';
    return `${baseUrl}${imageId}`;
  }
}
