import { Injectable } from '@angular/core';
import { RestApiService } from './rest-api.service';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class FrontEndLogService {
  constructor(private readonly api: RestApiService) {}

  public sendError(message: string, stack: string): Observable<any> {
    return this.api.post(`/frontend-log/error`, {
      message,
      stack,
    });
  }

  public sendWarning(message: string, stack: string): Observable<any> {
    return this.api.post(`/frontend-log/warning`, {
      message,
      stack,
    });
  }

  public sendInfo(message: string, stack: string): Observable<any> {
    return this.api.post(`/frontend-log/info`, {
      message,
      stack,
    });
  }
}
