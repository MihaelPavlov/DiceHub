import { Pipe, PipeTransform } from '@angular/core';
import { environment } from '../environments/environment.development';

@Pipe({
  name: 'rewardImage',
  pure: true,
})
export class RewardImagePipe implements PipeTransform {
  transform(imageId: number): string {
    const URL: string = `${environment.defaultAppUrl}/rewards/get-image/`;
    return `${URL}${imageId}`;
  }
}
