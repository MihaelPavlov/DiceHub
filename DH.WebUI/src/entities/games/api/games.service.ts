import { Injectable } from '@angular/core';
import { RestApiService } from '../../../shared/services/rest-api.service';
import { Observable } from 'rxjs';
import { PATH } from '../../../shared/configs/path.config';
import { IGameListResult } from '../models/game-list.model';
import { IGameByIdResult } from '../models/game-by-id.model';

@Injectable({
  providedIn: 'root',
})
export class GamesService {
  constructor(private readonly api: RestApiService) {}

  public getList(): Observable<IGameListResult[]> {
    return this.api.get<IGameListResult[]>(`/${PATH.GAMES.CORE}`);
  }

  public getById(id: number): Observable<IGameByIdResult> {
    return this.api.get<IGameByIdResult>(`/${PATH.GAMES.CORE}/${id}`);
  }
}
