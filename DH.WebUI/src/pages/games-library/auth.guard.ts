import { HttpClient, HttpHeaders } from "@angular/common/http";
import { ActivatedRouteSnapshot, CanActivate, Router, RouterStateSnapshot } from "@angular/router";
import { JwtHelperService } from "@auth0/angular-jwt";
import { AuthenticatedResponse } from "./page/games-library.component";
import { Injectable } from "@angular/core";

@Injectable({
    providedIn: 'root',
  })
  export class AuthGuard implements CanActivate {
    constructor(
      private router: Router,
      private jwtHelper: JwtHelperService,
      private http: HttpClient
    ) {}
  
    async  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot) {
      const token = localStorage.getItem('jwt');
      if (token && !this.jwtHelper.isTokenExpired(token)) {
        console.log(this.jwtHelper.decodeToken(token));
        return true;
      }
      const isRefreshSuccess = await this.tryRefreshingTokens(token);
      if (!isRefreshSuccess) {
         this.router.navigate(['games/library']);
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
      console.log("refresh");
      
      let isRefreshSuccess: boolean;
      const refreshRes = await new Promise<AuthenticatedResponse>(
        (resolve, reject) => {
          this.http
            .post<AuthenticatedResponse>(
              'https://localhost:7024/user/refresh',
              credentials,
              {
                headers: new HttpHeaders({
                  'Content-Type': 'application/json',
                }),
              }
            )
            .subscribe({
              next: (res: AuthenticatedResponse) => resolve(res),
              error: (_) => {
                reject;
                // REDIRECT TO LOGIN
                isRefreshSuccess = false;
              },
            });
        }
      );
      localStorage.setItem('jwt', refreshRes.accessToken);
      localStorage.setItem('refreshToken', refreshRes.refreshToken);
      isRefreshSuccess = true;
      return isRefreshSuccess;
    }
  }