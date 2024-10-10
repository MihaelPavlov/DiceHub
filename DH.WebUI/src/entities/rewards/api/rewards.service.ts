import { Observable } from 'rxjs';
import { RestApiService } from '../../../shared/services/rest-api.service';
import { PATH } from '../../../shared/configs/path.config';
import { Injectable } from '@angular/core';
import { ICreateRewardDto } from '../models/create-reward.model';
import { IRewardListResult } from '../models/reward-list.model';
import { IRewardGetByIdResult } from '../models/reward-by-id.model';
import { IUpdateRewardDto } from '../models/update-reward.model';
import { IUserChallengePeriodReward } from '../models/user-period-reward.model';

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

  public getById(id: number): Observable<IRewardGetByIdResult> {
    return this.api.get<IRewardGetByIdResult>(
      `/${PATH.REWARDS.CORE}/${PATH.REWARDS.SYSTEM_REWARD}/${id}`
    );
  }

  public getUserChallengePeriodRewardList(periodPerformanceId: number): Observable<IUserChallengePeriodReward[]> {
    return this.api.get<IUserChallengePeriodReward[]>(
      `/${PATH.REWARDS.CORE}/${PATH.REWARDS.GET_USER_REWARDS}/${periodPerformanceId}`
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

  public update(
    reward: IUpdateRewardDto,
    imageFile: File | null
  ): Observable<null> {
    const formData = new FormData();
    formData.append('reward', JSON.stringify(reward));
    if (imageFile) formData.append('imageFile', imageFile);

    return this.api.put(
      `/${PATH.REWARDS.CORE}/${PATH.REWARDS.SYSTEM_REWARD}`,
      formData
    );
  }

  public delete(id: number): Observable<null> {
    return this.api.delete(`/${PATH.REWARDS.CORE}/${PATH.REWARDS.SYSTEM_REWARD}/${id}`);
  }
}
