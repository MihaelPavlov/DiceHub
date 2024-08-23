import { Injectable } from '@angular/core';
import { RestApiService } from '../../../shared/services/rest-api.service';
import { Observable } from 'rxjs';
import { PATH } from '../../../shared/configs/path.config';
import { IGameListResult } from '../models/game-list.model';
import { IGameByIdResult } from '../models/game-by-id.model';
import { ICreateGameDto } from '../models/create-game.model';

@Injectable({
  providedIn: 'root',
})
export class GamesService {
  constructor(private readonly api: RestApiService) {}

  public getList(
    searchExpression: string = ''
  ): Observable<IGameListResult[] | null> {
    return this.api.post<IGameListResult[]>(
      `/${PATH.GAMES.CORE}/${PATH.GAMES.LIST}`,
      {
        searchExpression,
      }
    );
  }

  public getNewGameList(
    searchExpression: string = ''
  ): Observable<IGameListResult[] | null> {
    return this.api.post<IGameListResult[]>(
      `/${PATH.GAMES.CORE}/${PATH.GAMES.GET_NEW_GAMES}`,
      {
        searchExpression,
      }
    );
  }

  public getListByCategoryId(
    id: number,
    searchExpression: string = ''
  ): Observable<IGameListResult[] | null> {
    return this.api.post<IGameListResult[]>(
      `/${PATH.GAMES.CORE}/${PATH.GAMES.GET_BY_CATEGORY_ID}`,
      {
        id,
        searchExpression,
      }
    );
  }

  public add(
    game: ICreateGameDto,
    imageFile: File
  ): Observable<number | null> {
    const formData = new FormData();

  // Append the game data as a JSON string
  formData.append('game', JSON.stringify(game));

  // Append the image file
  formData.append('imageFile', imageFile);
    return this.api.post<number>(`/${PATH.GAMES.CORE}`, formData);
  }

  public getById(id: number): Observable<IGameByIdResult> {
    return this.api.get<IGameByIdResult>(`/${PATH.GAMES.CORE}/${id}`);
  }

  public likeGame(id: number): Observable<null> {
    return this.api.put(`/${PATH.GAMES.CORE}/${id}/${PATH.GAMES.LIKE}`, {});
  }

  public dislikeGame(id: number): Observable<null> {
    return this.api.put(`/${PATH.GAMES.CORE}/${id}/${PATH.GAMES.DISLIKE}`, {});
  }
}
