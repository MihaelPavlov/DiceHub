import { Injectable } from '@angular/core';
import { RestApiService } from '../../../shared/services/rest-api.service';
import { Observable } from 'rxjs';
import { PATH } from '../../../shared/configs/path.config';
import { IGameReviewListResult } from '../models/game-review-list.model';
import { IGameCreateDto } from '../models/create-game-review.model';
import { IGameUpdateDto } from '../models/update-game-review.model';

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
    return this.api.post<number>(`/${PATH.GAME_REVIEWS.CORE}`, { gameReview });
  }

  public update(updateGameReviewDto: IGameUpdateDto): Observable<null> {
    return this.api.put(`/${PATH.GAME_REVIEWS.CORE}`, { updateGameReviewDto });
  }

  public delete(id: number): Observable<null> {
    return this.api.delete(`/${PATH.GAME_REVIEWS.CORE}/${id}`);
  }
}
