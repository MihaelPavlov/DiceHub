import { ICreateChallengeDto } from './../models/create-challenge.model';
import { Observable } from 'rxjs';
import { RestApiService } from '../../../shared/services/rest-api.service';
import { PATH } from '../../../shared/configs/path.config';
import { Injectable } from '@angular/core';
import { IChallengeListResult } from '../models/challenge-list.model';
import { IChallengeResult } from '../models/challenge-by-id.model';
import { IUpdateChallengeDto } from '../models/update-challenge.model';
import { IUserChallenge } from '../models/user-challenge.model';
import { IUserChallengePeriodPerformance } from '../models/user-challenge-period-performance.model';
import { ICustomPeriod } from '../models/custom-period.model';
import { IUserCustomPeriod } from '../models/user-custom-period.model';
import { IUniversalChallengeListResult } from '../models/universal-challenge.model';
import { IUserUniversalChallenge } from '../models/user-universal-challenge.model';

@Injectable({
  providedIn: 'root',
})
export class ChallengesService {
  constructor(private readonly api: RestApiService) {}

  public getList(
    gameIds: number[] = []
  ): Observable<IChallengeListResult[] | null> {
    return this.api.post<IChallengeListResult[]>(
      `/${PATH.CHALLENGES.CORE}/${PATH.CHALLENGES.LIST}`,
      {
        gameIds,
      }
    );
  }

  public getUniversalList(): Observable<
    IUniversalChallengeListResult[] | null
  > {
    return this.api.get<IUniversalChallengeListResult[]>(
      `/${PATH.CHALLENGES.CORE}/${PATH.CHALLENGES.UNIVERSAL_LIST}`
    );
  }

  public getById(id: number): Observable<IChallengeResult> {
    return this.api.get<IChallengeResult>(`/${PATH.CHALLENGES.CORE}/${id}`);
  }

  public getUserChallengeList(): Observable<IUserChallenge[]> {
    return this.api.get<IUserChallenge[]>(
      `/${PATH.CHALLENGES.CORE}/${PATH.CHALLENGES.GET_USER_CHALLENGES}`
    );
  }

  public getUserUniversalChallengeList(): Observable<
    IUserUniversalChallenge[]
  > {
    return this.api.get<IUserUniversalChallenge[]>(
      `/${PATH.CHALLENGES.CORE}/${PATH.CHALLENGES.GET_USER_UNIVERSAL_CHALLENGES}`
    );
  }

  public getUserChallengePeriodPerformance(): Observable<IUserChallengePeriodPerformance | null> {
    return this.api.get<IUserChallengePeriodPerformance | null>(
      `/${PATH.CHALLENGES.CORE}/${PATH.CHALLENGES.GET_USER_CHALLENGE_PERIOD_PERFORMANCE}`
    );
  }

  public add(challenge: ICreateChallengeDto): Observable<number | null> {
    return this.api.post<number>(`/${PATH.CHALLENGES.CORE}`, {
      ...challenge,
    });
  }

  public update(challenge: IUpdateChallengeDto): Observable<null> {
    return this.api.put(`/${PATH.CHALLENGES.CORE}`, { ...challenge });
  }

  public delete(id: number): Observable<null> {
    return this.api.delete(`/${PATH.CHALLENGES.CORE}/${id}`);
  }

  public getCustomPeriod(): Observable<ICustomPeriod> {
    return this.api.get<ICustomPeriod>(
      `/${PATH.CHALLENGES.CORE}/${PATH.CHALLENGES.GET_CUSTOM_PERIOD}`
    );
  }

  public getUserCustomPeriod(): Observable<IUserCustomPeriod> {
    return this.api.get<IUserCustomPeriod>(
      `/${PATH.CHALLENGES.CORE}/${PATH.CHALLENGES.GET_USER_CUSTOM_PERIOD}`
    );
  }

  public saveCustomPeriod(customPeriod: ICustomPeriod): Observable<null> {
    return this.api.post(
      `/${PATH.CHALLENGES.CORE}/${PATH.CHALLENGES.SAVE_CUSTOM_PERIOD}`,
      { customPeriod }
    );
  }
}
