import { Observable } from 'rxjs';
import { RestApiService } from '../../../shared/services/rest-api.service';
import { IEventListResult } from '../models/event-list.model';
import { PATH } from '../../../shared/configs/path.config';
import { Injectable } from '@angular/core';
import { IEventByIdResult } from '../models/event-by-id.mode';
import { ICreateEventDto } from '../models/create-event.mode';
import { IUpdateEventDto } from '../models/update-event.model';
import { IEventDropdownListResult } from '../models/event-dropdown-list-result.model';

@Injectable({
  providedIn: 'root',
})
export class EventsService {
  constructor(private readonly api: RestApiService) {}

  public getList(
    searchExpression: string = ''
  ): Observable<IEventListResult[] | null> {
    return this.api.post<IEventListResult[]>(
      `/${PATH.EVENTS.CORE}/${PATH.EVENTS.LIST}`,
      {
        searchExpression,
      }
    );
  }

  public getAllEventsDropdownList(): Observable<IEventDropdownListResult[]> {
    return this.api.get<IEventDropdownListResult[]>(
      `/${PATH.EVENTS.CORE}/${PATH.EVENTS.GET_ALL_EVENTS_DROPDOWN_LIST}`
    );
  }

  public getById(id: number): Observable<IEventByIdResult> {
    return this.api.get<IEventByIdResult>(`/${PATH.EVENTS.CORE}/${id}`);
  }

  public participate(id: number): Observable<boolean | null> {
    return this.api.post<boolean>(
      `/${PATH.EVENTS.CORE}/${PATH.EVENTS.PARTICIPATE}`,
      { id }
    );
  }

  public removeParticipant(id: number): Observable<boolean | null> {
    return this.api.post<boolean>(
      `/${PATH.EVENTS.CORE}/${PATH.EVENTS.REMOVE_PARTICIPANT}`,
      { id }
    );
  }

  public checkUserParticipation(id: number): Observable<boolean | null> {
    return this.api.post<boolean>(
      `/${PATH.EVENTS.CORE}/${PATH.EVENTS.CHECK_USER_PARTICIPATION}`,
      { id }
    );
  }

  public add(
    eventDto: ICreateEventDto,
    customImageFile: File | null
  ): Observable<number | null> {
    const formData = new FormData();
    formData.append('eventModel', JSON.stringify(eventDto));
    if (customImageFile && eventDto.isCustomImage)
      formData.append('imageFile', customImageFile);

    return this.api.post<number>(`/${PATH.EVENTS.CORE}`, formData);
  }

  public update(
    eventDto: IUpdateEventDto,
    customImageFile: File | null
  ): Observable<number | null> {
    const formData = new FormData();
    formData.append('eventModel', JSON.stringify(eventDto));
    if (customImageFile && eventDto.isCustomImage)
      formData.append('imageFile', customImageFile);

    return this.api.put<number>(`/${PATH.EVENTS.CORE}`, formData);
  }
}
