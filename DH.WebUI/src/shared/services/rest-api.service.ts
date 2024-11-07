import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, map } from 'rxjs';
import { environment } from '../environments/environment.development';

const URL: string = `${environment.appUrl}`;

@Injectable({
  providedIn: 'root',
})
export class RestApiService {
  private readonly baseUrl;

  constructor(private readonly http: HttpClient) {
    this.baseUrl = URL;
  }

  public get<T>(path: string, options?: object): Observable<T> {
    return this.http
      .get<T>(`${this.baseUrl}${path}`, options)
      .pipe(map((result: any) => result as T));
  }

  public post<T>(
    path: string,
    body: object | string | number,
    options?: object
  ): Observable<T | null> {
    return this.http
      .post<T>(`${this.baseUrl}${path}`, body, options)
      .pipe(map((res: any) => res as T));
  }

  public put<T>(
    path: string,
    body: object | string,
    options?: object
  ): Observable<T | null> {
    return this.http
      .put<T>(`${this.baseUrl}${path}`, body, options)
      .pipe(map((res: any) => res as T));
  }

  public delete<T>(path: string, options?: object): Observable<any> {
    return this.http
      .delete<T>(`${this.baseUrl}${path}`, options)
      .pipe(map((res: any) => res as T));
  }
}
