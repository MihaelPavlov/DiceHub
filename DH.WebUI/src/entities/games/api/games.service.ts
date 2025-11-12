import { Injectable } from '@angular/core';
import { RestApiService } from '../../../shared/services/rest-api.service';
import { Observable } from 'rxjs';
import { PATH } from '../../../shared/configs/path.config';
import { IGameListResult } from '../models/game-list.model';
import { IGameByIdResult } from '../models/game-by-id.model';
import { ICreateGameDto } from '../models/create-game.model';
import { IUpdateGameDto } from '../models/update-game.model';
import { IGameDropdownResult } from '../models/game-dropdown.model';
import { IGameInventory } from '../models/game-inventory.mode';
import { ICreateGameReservation } from '../models/create-game-reservation.model';
import { IReservedGame } from '../models/reserved-game.model';
import { IGameReservationStatus } from '../models/game-reservation-status.model';
import { ActiveReservedGame } from '../models/active-reserved-game.model';
import { IGetReservationById } from '../models/get-reservation-by-id.model';
import { IGameReservationHistory } from '../models/game-reservation-history.model';
import { ReservationStatus } from '../../../shared/enums/reservation-status.enum';

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

  public getReservationById(id: number): Observable<IGetReservationById> {
    return this.api.get<IGetReservationById>(
      `/${PATH.GAMES.CORE}/${PATH.GAMES.GET_RESERVATION_BY_ID}/${id}`
    );
  }

  public getReservationHistory(
    status: ReservationStatus | null = null
  ): Observable<IGameReservationHistory[] | null> {
    return this.api.post<IGameReservationHistory[]>(
      `/${PATH.GAMES.CORE}/${PATH.GAMES.GET_RESERVATION_HISTORY}`,
      { status }
    );
  }

  public generateQRCode(qrCodeString: string): Observable<any> {
    console.log(qrCodeString);

    return this.api.post(`/${PATH.GAMES.CORE}/create-qr-code`, {
      qrCodeData: qrCodeString,
    });
  }

  public upload(data: any): Observable<any> {
    return this.api.post<any>(`/${PATH.GAMES.CORE}/upload`, {
      imageData: data,
    });
  }

  public getInventory(id: number): Observable<IGameInventory> {
    return this.api.get<IGameInventory>(
      `/${PATH.GAMES.CORE}/${id}/${PATH.GAMES.INVENTORY}`
    );
  }

  public getReservations(): Observable<IReservedGame[]> {
    return this.api.get<IReservedGame[]>(
      `/${PATH.GAMES.CORE}/${PATH.GAMES.GET_RESERVED_GAMES}`
    );
  }

  public getReservations_BackgroundRequest(): Observable<IReservedGame[]> {
    return this.api.get<IReservedGame[]>(
      `/${PATH.GAMES.CORE}/${PATH.GAMES.GET_RESERVED_GAMES}`,
      { backgroundRequest: true }
    );
  }

  public getActiveReservation(): Observable<ActiveReservedGame> {
    return this.api.get<ActiveReservedGame>(
      `/${PATH.GAMES.CORE}/${PATH.GAMES.GET_ACTIVE_RESERVED_GAME}`
    );
  }

  public getActiveReservations(): Observable<ActiveReservedGame[]> {
    return this.api.get<ActiveReservedGame[]>(
      `/${PATH.GAMES.CORE}/${PATH.GAMES.GET_ACTIVE_RESERVED_GAMES}`
    );
  }

  public getActiveReservationsCount(): Observable<number> {
    return this.api.get<number>(
      `/${PATH.GAMES.CORE}/${PATH.GAMES.GET_ACTIVE_RESERVED_GAMES_COUNT}`
    );
  }

  public getDropdownList(): Observable<IGameDropdownResult[]> {
    return this.api.get<IGameDropdownResult[]>(
      `/${PATH.GAMES.CORE}/${PATH.GAMES.GET_DROPDOWN_LIST}`
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

  public add(game: ICreateGameDto, imageFile: File): Observable<number | null> {
    const formData = new FormData();
    formData.append('game', JSON.stringify(game));
    formData.append('imageFile', imageFile);

    return this.api.post<number>(`/${PATH.GAMES.CORE}`, formData);
  }

  public addCopy(id: number): Observable<null> {
    return this.api.post(`/${PATH.GAMES.CORE}/${PATH.GAMES.COPY}`, { id });
  }

  public delete(id: number): Observable<null> {
    return this.api.delete(`/${PATH.GAMES.CORE}/${id}`);
  }

  public update(
    game: IUpdateGameDto,
    imageFile: File | null
  ): Observable<null> {
    const formData = new FormData();
    formData.append('game', JSON.stringify(game));
    if (imageFile) formData.append('imageFile', imageFile);

    return this.api.put(`/${PATH.GAMES.CORE}`, formData);
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

  public reservation(reservation: ICreateGameReservation): Observable<null> {
    return this.api.post(`/${PATH.GAMES.CORE}/${PATH.GAMES.RESERVATION}`, {
      reservation,
    });
  }

  public reservationStatus(
    id: number
  ): Observable<IGameReservationStatus | null> {
    return this.api.post(
      `/${PATH.GAMES.CORE}/${PATH.GAMES.RESERVATION_STATUS}`,
      {
        id,
      }
    );
  }

  public userReservationStatus(): Observable<IGameReservationStatus | null> {
    return this.api.post(
      `/${PATH.GAMES.CORE}/${PATH.GAMES.USER_RESERVATION_STATUS}`,
      {}
    );
  }

  public approveReservation(
    reservationId: number,
    publicNote: string,
    internalNote: string
  ): Observable<null> {
    return this.api.put(
      `/${PATH.GAMES.CORE}/${PATH.GAMES.APPROVE_RESERVATION}`,
      {
        id: reservationId,
        publicNote,
        internalNote,
      }
    );
  }

  public declinedReservation(
    reservationId: number,
    publicNote: string,
    internalNote: string
  ): Observable<null> {
    return this.api.put(
      `/${PATH.GAMES.CORE}/${PATH.GAMES.DECLINE_RESERVATION}`,
      {
        id: reservationId,
        publicNote,
        internalNote,
      }
    );
  }

  public cancelReservation(reservationId: number): Observable<null> {
    return this.api.put(
      `/${PATH.GAMES.CORE}/${PATH.GAMES.CANCEL_RESERVATION}`,
      {
        id: reservationId,
      }
    );
  }

  public updateReservation(
    id: number,
    publicNote: string,
    internalNote: string
  ): Observable<null> {
    return this.api.put(
      `/${PATH.GAMES.CORE}/${PATH.GAMES.UPDATE_RESERVATION}`,
      {
        id,
        publicNote,
        internalNote,
      }
    );
  }

  public deleteReservation(id: number): Observable<null> {
    return this.api.delete(
      `/${PATH.GAMES.CORE}/${PATH.GAMES.DELETE_RESERVATION}/${id}`
    );
  }
}
