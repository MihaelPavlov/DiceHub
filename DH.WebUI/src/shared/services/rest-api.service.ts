import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, map } from 'rxjs';
import { environment } from '../environments/environment.development';

export enum ApiBase {
  Default = 'default',
  Statistics = 'statistics',
}
export const ApiEndpoints = {
  default: `${environment.defaultAppUrl}`,
  statistics: `${environment.statisticsAppUrl}`,
};

interface ApiConfig {
  options?: object;
  base?: ApiBase;
}

@Injectable({
  providedIn: 'root',
})
export class RestApiService {
  constructor(private readonly http: HttpClient) {}

  private getBaseUrl(base: ApiBase): string {
    return ApiEndpoints[base] || ApiEndpoints.default;
  }

  private buildUrl(base: ApiBase, path: string): string {
    return `${this.getBaseUrl(base)}${path}`;
  }

  public get<T>(
    path: string,
    config: ApiConfig = {},
    backgroundRequest: boolean = false
  ): Observable<T> {
    let { options = {}, base = ApiBase.Default } = config;

    if (backgroundRequest) {
      const updatedHeaders = new HttpHeaders().set(
        'X-Background-Request',
        'true'
      );
      options = {
        ...options,
        headers: updatedHeaders,
      };
    }

    return this.http
      .get<T>(this.buildUrl(base, path), options)
      .pipe(map((result: any) => result as T));
  }

  public post<T>(
    path: string,
    body: object | string | number,
    config: ApiConfig = {}
  ): Observable<T | null> {
    const { options = {}, base = ApiBase.Default } = config;

    return this.http
      .post<T>(this.buildUrl(base, path), body, options)
      .pipe(map((res: any) => res as T));
  }

  public put<T>(
    path: string,
    body: object | string,
    config: ApiConfig = {}
  ): Observable<T | null> {
    const { options = {}, base = ApiBase.Default } = config;

    return this.http
      .put<T>(this.buildUrl(base, path), body, options)
      .pipe(map((res: any) => res as T));
  }

  public delete<T>(path: string, config: ApiConfig = {}): Observable<any> {
    const { options = {}, base = ApiBase.Default } = config;

    return this.http
      .delete<T>(this.buildUrl(base, path), options)
      .pipe(map((res: any) => res as T));
  }
}
