import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'gameImage',
  pure: true,
})
export class GameImagePipe implements PipeTransform {
  transform(imageId: number): string {
    const baseUrl = 'https://localhost:7024/games/get-image/';
    return `${baseUrl}${imageId}`;
  }
}
