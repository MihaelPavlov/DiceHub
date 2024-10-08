import { HttpClient, HttpHeaders } from '@angular/common/http';
import {
  ActivatedRouteSnapshot,
  CanActivate,
  Router,
  RouterStateSnapshot,
} from '@angular/router';
import { JwtHelperService } from '@auth0/angular-jwt';
import { Injectable } from '@angular/core';
import { AuthenticatedResponse } from '../../entities/auth/auth.service';

@Injectable({
  providedIn: 'root',
})
export class AuthGuard implements CanActivate {
  constructor(
    private readonly router: Router,
    private readonly jwtHelper: JwtHelperService,
    private readonly http: HttpClient
  ) {}

  async canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot) {
    const token = localStorage.getItem('jwt');
    if (token && !this.jwtHelper.isTokenExpired(token)) {
      console.log(this.jwtHelper.decodeToken(token));
      return true;
    }
    const isRefreshSuccess = await this.tryRefreshingTokens(token);
    console.log('refresh',isRefreshSuccess);

    if (!isRefreshSuccess) {
      this.router.navigate(['login']);
    }
    return isRefreshSuccess;
  }

  private async tryRefreshingTokens(token: string | null): Promise<boolean> {
    const refreshToken: string | null = localStorage.getItem('refreshToken');
    if (!token || !refreshToken) {
      return false;
    }

    const credentials = JSON.stringify({
      accessToken: token,
      refreshToken: refreshToken,
    });
    console.log('refresh');

    let isRefreshSuccess: boolean = true;
    try{
      const refreshRes = await new Promise<AuthenticatedResponse>(
        (resolve, reject) => {
          this.http
            .post<AuthenticatedResponse>(
              'https://localhost:7024/user/refresh',
              {
                accessToken: token,
                refreshToken: refreshToken,
              },
              {
                headers: new HttpHeaders({
                  'Content-Type': 'application/json',
                }),
              }
            )
            .subscribe({
              next: (res: AuthenticatedResponse) => resolve(res),
              error: (_) => {
                console.log(_);
                
               reject();
              },
            });
        }
      );    

        localStorage.setItem('jwt', refreshRes.accessToken);
        localStorage.setItem('refreshToken', refreshRes.refreshToken);
      
        isRefreshSuccess = true;
      
    }
    catch{
      isRefreshSuccess=false;
    }
    

    return isRefreshSuccess;
  }
}
