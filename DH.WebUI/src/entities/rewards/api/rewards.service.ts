import { Observable } from 'rxjs';
import { RestApiService } from '../../../shared/services/rest-api.service';
import { PATH } from '../../../shared/configs/path.config';
import { Injectable } from '@angular/core';
import { ICreateRewardDto } from '../models/create-reward.model';
import { IRewardListResult } from '../models/reward-list.model';

@Injectable({
  providedIn: 'root',
})
export class RewardsService {
  constructor(private readonly api: RestApiService) {}

  public getList(
    searchExpression: string = ''
  ): Observable<IRewardListResult[] | null> {
    return this.api.post<IRewardListResult[]>(
      `/${PATH.REWARDS.CORE}/${PATH.REWARDS.SYSTEM_REWARD_LIST}`,
      {
        searchExpression,
      }
    );
  }

  public add(
    reward: ICreateRewardDto,
    imageFile: File
  ): Observable<number | null> {
    const formData = new FormData();
    formData.append('reward', JSON.stringify(reward));
    formData.append('imageFile', imageFile);

    return this.api.post<number>(
      `/${PATH.REWARDS.CORE}/${PATH.REWARDS.SYSTEM_REWARD}`,
      formData
    );
  }
}
