import { Injectable } from '@angular/core';
import { RestApiService } from '../../../shared/services/rest-api.service';
import { Observable } from 'rxjs';
import { PATH } from '../../../shared/configs/path.config';
import { IGameReviewListResult } from '../models/game-review-list.model';
import { IGameCreateDto } from '../models/create-game-review.model';

@Injectable({
  providedIn: 'root',
})
export class GameReviewsService {
  constructor(private readonly api: RestApiService) {}

  public getList(gameId: number): Observable<IGameReviewListResult[] | null> {
    return this.api.get<IGameReviewListResult[]>(
      `/${PATH.GAME_REVIEWS.CORE}/${gameId}`
    );
  }

  public create(gameReview: IGameCreateDto): Observable<number | null> {
    return this.api.post<number>(`/${PATH.GAME_REVIEWS.CORE}`, {gameReview});
  }
}
