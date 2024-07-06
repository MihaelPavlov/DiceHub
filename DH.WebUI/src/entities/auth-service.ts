import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
export interface IUserInfo {
  id: string;
  role: string;
}
@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private readonly userInfoSubject$: BehaviorSubject<IUserInfo | null> =
    new BehaviorSubject<IUserInfo | null>(null);

  public userInfo$ = this.userInfoSubject$.asObservable();
  constructor(readonly httpClient: HttpClient) {
    this.userinfo();
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
      });
  }

  game(loginForm: any) {
    const token = localStorage.getItem('jwt');

    const httpsOptions = {
      headers: new HttpHeaders({
        'Content-Type': 'application/json',
        Authorization: `Bearer ${token}`,
      }),
    };
    return this.httpClient
      .post<any>('https://localhost:7024/games', loginForm, httpsOptions)
      .subscribe((response) => {});
  }

  register(registerForm: any) {
    return this.httpClient
      .post<any>('https://localhost:7024/user/register', registerForm)
      .subscribe((_) => {});
  }

  userinfo() {
    const token = localStorage.getItem('jwt');

    const httpsOptions = {
      headers: new HttpHeaders({
        'Content-Type': 'application/json',
        Authorization: `Bearer ${token}`,
      }),
    };
    return this.httpClient
      .post('https://localhost:7024/user/info', {}, httpsOptions)
      .subscribe((user: any) => {
        if (user) this.userInfoSubject$.next({ id: user.id, role: user.role });
      });
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
