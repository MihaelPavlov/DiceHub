import { ICreateRewardDto } from './create-reward.model';

export interface IUpdateRewardDto extends ICreateRewardDto {
  id: number;
  imageUrl: string | null;
}
