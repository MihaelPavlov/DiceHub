import { Observable } from 'rxjs';
import { RestApiService } from '../../../shared/services/rest-api.service';
import { IEventListResult } from '../models/event-list.model';
import { PATH } from '../../../shared/configs/path.config';
import { Injectable } from '@angular/core';
import { IEventByIdResult } from '../models/event-by-id.mode';
import { ICreateEventDto } from '../models/create-event.mode';

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

  public getById(id: number): Observable<IEventByIdResult> {
    return this.api.get<IEventByIdResult>(`/${PATH.EVENTS.CORE}/${id}`);
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
}
