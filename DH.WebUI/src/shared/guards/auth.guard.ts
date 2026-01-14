import { AuthService } from './../../entities/auth/auth.service';
import { HttpHeaders } from '@angular/common/http';
import {
  ActivatedRouteSnapshot,
  Router,
  ROUTER_CONFIGURATION,
  RouterStateSnapshot,
} from '@angular/router';
import { JwtHelperService } from '@auth0/angular-jwt';
import { Injectable } from '@angular/core';
import { ITokenResponse } from '../../entities/auth/models/token-response.model';
import {
  catchError,
  from,
  map,
  Observable,
  of,
  switchMap,
  take,
  tap,
} from 'rxjs';
import { RestApiService } from '../services/rest-api.service';
import { TenantContextService } from '../services/tenant-context.service';
import { TenantRouter } from '../helpers/tenant-router';
import { PATH } from '../configs/path.config';
import { ROUTE } from '../configs/route.config';

@Injectable({
  providedIn: 'root',
})
export class AuthGuard {
  constructor(
    private readonly router: Router,
    private readonly tenantRouter: TenantRouter,
    private readonly jwtHelper: JwtHelperService,
    private readonly api: RestApiService,
    private readonly authService: AuthService
  ) {}

  public canActivateChild(
    childRoute: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): Observable<boolean> | boolean | Promise<boolean> {
    return this.canActivate(childRoute, state); // reuse the same logic
  }

  public canActivate(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): Observable<boolean> | boolean | Promise<boolean> {
    const token = localStorage.getItem('jwt');

    if (!token) {
      this.authService.userInfoSubject$.next(null);
      this.tenantRouter.navigateTenant(ROUTE.LOGIN);
      return false;
    }

    if (!this.jwtHelper.isTokenExpired(token)) return true;

    return this.tryRefreshingTokens(token).pipe(
      take(1),
      switchMap((isRefreshSuccess) => {
        if (!isRefreshSuccess) {
          // logout is already an Observable, chain it
          return this.authService.logout().pipe(
            tap(() => this.tenantRouter.navigateTenant(ROUTE.LOGIN)),
            map(() => false) // emit false after logout
          );
        } else {
          // refresh succeeded, load user info
          return from(this.authService.userinfo$()).pipe(
            map(() => true),
            catchError(() => {
              return of(false);
            })
          );
        }
      })
    );
  }

  private tryRefreshingTokens(token: string | null): Observable<boolean> {
    const refreshToken: string | null = localStorage.getItem('refreshToken');
    if (!token || !refreshToken) {
      return this.authService.logout().pipe(map(() => false));
    }

    const credentials = {
      accessToken: token,
      refreshToken: refreshToken,
    };
    let headers = new HttpHeaders({ 'Content-Type': 'application/json' });

    return this.api
      .post<ITokenResponse>(`/api/user/refresh`, credentials, {
        options: { headers },
      })
      .pipe(
        take(1),
        tap((res: ITokenResponse | null) => {
          if (res) {
            localStorage.setItem('jwt', res.accessToken);
            localStorage.setItem('refreshToken', res.refreshToken);
          }
        }),
        map(() => true), // Emit true if refresh is successful
        catchError((error) => {
          console.error(error);
          return this.authService.logout(true).pipe(map(() => false));
        })
      );
  }
}
