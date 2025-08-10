import { Injectable } from '@angular/core';
import { RestApiService } from '../../../shared/services/rest-api.service';
import { PATH } from '../../../shared/configs/path.config';
import { Observable } from 'rxjs';
import { IGameCategory } from '../models/game-category.model';

@Injectable({
  providedIn: 'root',
})
export class GameCategoriesService {
  constructor(private readonly api: RestApiService) {}

  public getList(
    searchExpression: string = ''
  ): Observable<IGameCategory[] | null> {
    return this.api.post<IGameCategory[]>(`/${PATH.GAME_CATEGORIES.CORE}/${PATH.GAME_CATEGORIES.LIST}`, {
      searchExpression,
    });
  }
}
