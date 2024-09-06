import { Injectable } from '@angular/core';
import { RestApiService } from '../../../shared/services/rest-api.service';
import { ICreateRoomDto } from '../models/create-room.model';
import { Observable } from 'rxjs';
import { PATH } from '../../../shared/configs/path.config';
import { IRoomListResult } from '../models/room-list.model';
import { IRoomByIdResult } from '../models/room-by-id.model';

@Injectable({
  providedIn: 'root',
})
export class RoomsService {
  constructor(private readonly api: RestApiService) {}

  public getList(
    searchExpression: string = ''
  ): Observable<IRoomListResult[] | null> {
    return this.api.post<IRoomListResult[]>(
      `/${PATH.ROOMS.CORE}/${PATH.ROOMS.LIST}`,
      {
        searchExpression,
      }
    );
  }

  public getById(id: number): Observable<IRoomByIdResult> {
    return this.api.get<IRoomByIdResult>(`/${PATH.ROOMS.CORE}/${id}`);
  }

  public add(room: ICreateRoomDto): Observable<number | null> {
    return this.api.post<number>(`/${PATH.ROOMS.CORE}`, { room });
  }
}
