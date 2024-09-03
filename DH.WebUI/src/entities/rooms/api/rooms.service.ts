import { Injectable } from '@angular/core';
import { RestApiService } from '../../../shared/services/rest-api.service';
import { ICreateRoomDto } from '../models/create-room.model';
import { Observable } from 'rxjs';
import { PATH } from '../../../shared/configs/path.config';

@Injectable({
  providedIn: 'root',
})
export class RoomsService {
  constructor(private readonly api: RestApiService) {}

  public add(room: ICreateRoomDto): Observable<number | null> {
    return this.api.post<number>(`/${PATH.ROOMS.CORE}`, { room });
  }
}
