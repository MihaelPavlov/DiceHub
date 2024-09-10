import { Injectable } from '@angular/core';
import { RestApiService } from '../../../shared/services/rest-api.service';
import { IAddUpdateRoomDto } from '../models/add-update-room.model';
import { Observable } from 'rxjs';
import { PATH } from '../../../shared/configs/path.config';
import { IRoomListResult } from '../models/room-list.model';
import { IRoomByIdResult } from '../models/room-by-id.model';
import { IRoomMessageResult } from '../models/room-message.model';
import { IRoomMemberResult } from '../models/room-member.model';
import { IRoomInfoMessageResult } from '../models/room-info-message.model';

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

  public getMembers(
    id: number,
    searchExpression: string = ''
  ): Observable<IRoomMemberResult[] | null> {
    return this.api.post<IRoomMemberResult[]>(
      `/${PATH.ROOMS.CORE}/${PATH.ROOMS.MEMBER_LIST}`,
      {
        id,
        searchExpression,
      }
    );
  }

  public removeMember(id: number, userId: string): Observable<null> {
    return this.api.post(`/${PATH.ROOMS.CORE}/${PATH.ROOMS.REMOVE_MEMBER}`, {
      id,
      userId,
    });
  }

  public getMessageList(roomId: number): Observable<IRoomMessageResult[]> {
    return this.api.get<IRoomMessageResult[]>(
      `/${PATH.ROOMS.CORE}/${roomId}/${PATH.ROOMS.MESSAGE_LIST}`
    );
  }

  public getInfoMessageList(roomId: number): Observable<IRoomInfoMessageResult[]> {
    return this.api.get<IRoomInfoMessageResult[]>(
      `/${PATH.ROOMS.CORE}/${roomId}/${PATH.ROOMS.INFO_MESSAGE_LIST}`
    );
  }

  public checkUserParticipateInRoom(roomId: number): Observable<boolean> {
    return this.api.get<boolean>(
      `/${PATH.ROOMS.CORE}/${roomId}/${PATH.ROOMS.CHECK_USER_PARTICIPATION}`
    );
  }

  public getById(id: number): Observable<IRoomByIdResult> {
    return this.api.get<IRoomByIdResult>(`/${PATH.ROOMS.CORE}/${id}`);
  }

  public add(room: IAddUpdateRoomDto): Observable<number | null> {
    return this.api.post<number>(`/${PATH.ROOMS.CORE}`, { room });
  }

  public update(room: IAddUpdateRoomDto): Observable<null> {
    return this.api.put(`/${PATH.ROOMS.CORE}`, { room });
  }

  public join(id: number): Observable<null> {
    return this.api.post(`/${PATH.ROOMS.CORE}/${PATH.ROOMS.JOIN}`, { id });
  }

  public leave(id: number): Observable<null> {
    return this.api.post(`/${PATH.ROOMS.CORE}/${PATH.ROOMS.LEAVE}`, { id });
  }

  public delete(id: number): Observable<null> {
    return this.api.delete(`/${PATH.ROOMS.CORE}/${id}`);
  }
}
