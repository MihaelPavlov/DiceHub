import { Pipe, PipeTransform } from '@angular/core';
import { environment } from '../environments/environment.development';

@Pipe({
  name: 'gameImage',
  pure: true,
})
export class GameImagePipe implements PipeTransform {
  transform(imageId: number): string {
    const URL: string = `${environment.defaultAppUrl}/games/get-image/`;
    return `${URL}${imageId}`;
  }
}
