import { TranslateService } from '@ngx-translate/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, map } from 'rxjs';
import { environment } from '../environments/environment.development';
import { LanguageService } from './language.service';

export enum ApiBase {
  Default = 'default',
  Statistics = 'statistics',
}
export const ApiEndpoints = {
  default: `${environment.defaultAppUrl}`,
};

interface ApiConfig {
  options?: {
    headers?: HttpHeaders;
    [key: string]: any;
  };
  base?: ApiBase;
  backgroundRequest?: boolean;
}

@Injectable({
  providedIn: 'root',
})
export class RestApiService {
  constructor(
    private readonly http: HttpClient,
    private readonly translateService: TranslateService
  ) {}

  private getBaseUrl(base: ApiBase): string {
    return ApiEndpoints[base] || ApiEndpoints.default;
  }

  private buildUrl(base: ApiBase, path: string): string {
    return `${this.getBaseUrl(base)}${path}`;
  }

  public get<T>(path: string, config: ApiConfig = {}): Observable<T> {
    const { base = ApiBase.Default } = config;
    const options = this.buildRequestOptions(config);

    return this.http
      .get<T>(this.buildUrl(base, path), options)
      .pipe(map((result: any) => result as T));
  }

  public post<T>(
    path: string,
    body: object | string | number,
    config: ApiConfig = {}
  ): Observable<T | null> {
    const { base = ApiBase.Default } = config;
    const options = this.buildRequestOptions(config);

    return this.http
      .post<T>(this.buildUrl(base, path), body, options)
      .pipe(map((res: any) => res as T));
  }

  public put<T>(
    path: string,
    body: object | string,
    config: ApiConfig = {}
  ): Observable<T | null> {
    const { base = ApiBase.Default } = config;
    const options = this.buildRequestOptions(config);

    return this.http
      .put<T>(this.buildUrl(base, path), body, options)
      .pipe(map((res: any) => res as T));
  }

  public delete<T>(path: string, config: ApiConfig = {}): Observable<any> {
    const { base = ApiBase.Default } = config;
    const options = this.buildRequestOptions(config);

    return this.http
      .delete<T>(this.buildUrl(base, path), options)
      .pipe(map((res: any) => res as T));
  }

  private buildRequestOptions(config: ApiConfig): any {
    let headers = config.options?.headers || new HttpHeaders();

    if (config.backgroundRequest) {
      headers = headers.set('X-Background-Request', 'true');
    }

    if (this.translateService.getCurrentLang())
      headers = headers.set('Accept-Language', this.translateService.getCurrentLang());

    return {
      ...config.options,
      headers,
    };
  }
}
