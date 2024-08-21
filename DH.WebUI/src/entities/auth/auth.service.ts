import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, map } from 'rxjs';
import { IUserInfo } from './models/user-info.model';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private readonly userInfoSubject$: BehaviorSubject<IUserInfo | null> =
    new BehaviorSubject<IUserInfo | null>(null);

  public userInfo$ = this.userInfoSubject$.asObservable();

  constructor(readonly httpClient: HttpClient) {}

  public get getUser(): IUserInfo | null {
    return this.userInfoSubject$.value;
  }

  login(loginForm: any) {
    const httpsOptions = {
      headers: new HttpHeaders({ 'Content-Type': 'application/json' }),
    };
    return this.httpClient
      .post<any>('https://localhost:7024/user', loginForm, httpsOptions)
      .subscribe((response) => {
        const accessToken = response.accessToken;
        const refreshToken = response.refreshToken;
        localStorage.setItem('jwt', accessToken);
        localStorage.setItem('refreshToken', refreshToken);
        this.userinfo();
      });
  }

  // For tests
  game(gameForm: any) {
    return this.httpClient
      .post<any>('https://localhost:7024/games', gameForm)
      .subscribe((response) => {
        console.log(response);
      });
  }

  register(registerForm: any) {
    return this.httpClient
      .post<any>('https://localhost:7024/user/register', registerForm)
      .subscribe((_) => {
        console.log(_);
      });
  }

  //TODO: Change it to private in future
  public userinfo(): void {
    const sidClaim: string =
      'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/sid';
    const roleClaim: string =
      'http://schemas.microsoft.com/ws/2008/06/identity/claims/role';

    this.httpClient
      .get('https://localhost:7024/user/info')
      .subscribe((user: any) => {
        if (user)
          this.userInfoSubject$.next({
            id: user[sidClaim],
            role: user[roleClaim],
            permissionString: user['permissions'],
          });

        console.log(this.userInfoSubject$.value);
      });
  }

  public isAuthenticated(): Observable<boolean> {
    return this.userInfo$.pipe(
      map((user) => {
        return user ? true : false;
      })
    );
  }

  logout() {
    localStorage.removeItem('jwt');
    localStorage.removeItem('refreshToken');
  }
}

export interface AuthenticatedResponse {
  accessToken: string;
  refreshToken: string;
}
