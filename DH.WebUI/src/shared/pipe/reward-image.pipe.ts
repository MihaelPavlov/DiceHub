import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'rewardImage',
  pure: true,
})
export class RewardImagePipe implements PipeTransform {
  transform(imageId: number): string {
    const baseUrl = 'https://localhost:7024/rewards/get-image/';
    return `${baseUrl}${imageId}`;
  }
}
