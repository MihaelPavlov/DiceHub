import { ICreateChallengeDto } from './create-challenge.model';

export interface IUpdateChallengeDto extends ICreateChallengeDto {
  id: number;
}
