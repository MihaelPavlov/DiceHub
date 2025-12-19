import { TranslateService } from '@ngx-translate/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, map } from 'rxjs';
import { environment } from '../environments/environment.development';
import { TenantContextService } from './tenant-context.service';

export enum ApiBase {
  Default = 'default',
  Statistics = 'statistics',
}
export const ApiEndpoints = {
  default: `${environment.defaultAppUrl}`,
};

export interface ApiConfig {
  options?: {
    headers?: HttpHeaders;
    [key: string]: any;
  };
  base?: ApiBase;
  backgroundRequest?: boolean;
  requiredTenant?: boolean;
}

@Injectable({
  providedIn: 'root',
})
export class RestApiService {
  constructor(
    private readonly http: HttpClient,
    private readonly translateService: TranslateService,
    private readonly tenantContextService: TenantContextService
  ) {}

  private getBaseUrl(base: ApiBase): string {
    return ApiEndpoints[base] || ApiEndpoints.default;
  }
  private normalizePath(path: string): string {
    return path.startsWith('/') ? path : `/${path}`;
  }
  private shouldUseTenant(config: ApiConfig): boolean {
    return (
      config.requiredTenant !== undefined && config.requiredTenant !== false
    );
  }

  private buildUrl(config: ApiConfig, path: string): string {
    const base = this.getBaseUrl(config.base || ApiBase.Default);
    const normalizedPath = this.normalizePath(path);

    const useTenant = this.shouldUseTenant(config);
    const tenantId = this.tenantContextService.tenantId;

    if (useTenant && !tenantId) {
      throw new Error('Tenant required but missing');
    }

    const apiPrefix = normalizedPath.startsWith('/api') ? '' : '/api';

    const tenantPrefix = useTenant && tenantId ? `/${tenantId}` : '';

    return `${base}${apiPrefix}${tenantPrefix}${normalizedPath}`;
  }

  public get<T>(path: string, config: ApiConfig = {}): Observable<T> {
    const options = this.buildRequestOptions(config);

    return this.http
      .get<T>(this.buildUrl(config, path), options)
      .pipe(map((result: any) => result as T));
  }

  public post<T>(
    path: string,
    body: object | string | number,
    config: ApiConfig = {}
  ): Observable<T | null> {
    const options = this.buildRequestOptions(config);

    return this.http
      .post<T>(this.buildUrl(config, path), body, options)
      .pipe(map((res: any) => res as T));
  }

  public put<T>(
    path: string,
    body: object | string,
    config: ApiConfig = {}
  ): Observable<T | null> {
    const options = this.buildRequestOptions(config);

    return this.http
      .put<T>(this.buildUrl(config, path), body, options)
      .pipe(map((res: any) => res as T));
  }

  public delete<T>(path: string, config: ApiConfig = {}): Observable<any> {
    const options = this.buildRequestOptions(config);

    return this.http
      .delete<T>(this.buildUrl(config, path), options)
      .pipe(map((res: any) => res as T));
  }

  private buildRequestOptions(config: ApiConfig): any {
    let headers = config.options?.headers || new HttpHeaders();

    if (config.backgroundRequest) {
      headers = headers.set('X-Background-Request', 'true');
    }

    if (config.requiredTenant !== null)
      headers = headers.set(
        'X-Requires-Tenant',
        config.requiredTenant ? 'true' : 'false'
      );

    if (this.translateService.getCurrentLang())
      headers = headers.set(
        'Accept-Language',
        this.translateService.getCurrentLang()
      );

    return {
      ...config.options,
      headers,
    };
  }
}
