import { AuthService } from './../../entities/auth/auth.service';
import { HttpHeaders } from '@angular/common/http';
import {
  ActivatedRouteSnapshot,
  Router,
  RouterStateSnapshot,
} from '@angular/router';
import { JwtHelperService } from '@auth0/angular-jwt';
import { Injectable } from '@angular/core';
import { ITokenResponse } from '../../entities/auth/models/token-response.model';
import { catchError, map, Observable, of, take, tap } from 'rxjs';
import { RestApiService } from '../services/rest-api.service';

@Injectable({
  providedIn: 'root',
})
export class AuthGuard {
  constructor(
    private readonly router: Router,
    private readonly jwtHelper: JwtHelperService,
    private readonly api: RestApiService,
    private readonly authService: AuthService
  ) {}

  public canActivate(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): Observable<boolean> | boolean | Promise<boolean> {
    const token = localStorage.getItem('jwt');
    
    if (token && !this.jwtHelper.isTokenExpired(token)) {
      return of(true);
    }

    return this.tryRefreshingTokens(token).pipe(
      take(1),
      tap((isRefreshSuccess) => {
        if (!isRefreshSuccess) {
          this.authService.logout();
          this.router.navigateByUrl('login');
        } else {
          this.authService.userinfo();
        }
      })
    );
  }

  private tryRefreshingTokens(token: string | null): Observable<boolean> {
    const refreshToken: string | null = localStorage.getItem('refreshToken');
    if (!token || !refreshToken) {
      return of(false);
    }

    const credentials = {
      accessToken: token,
      refreshToken: refreshToken,
    };

    return this.api
      .post<ITokenResponse>(`/api/user/refresh`, credentials, {
        options: {
          headers: new HttpHeaders({ 'Content-Type': 'application/json' }),
        },
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
          this.authService.logout();
          return of(false); // Emit false if refresh fails
        })
      );
  }
}
