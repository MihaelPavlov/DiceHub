import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  constructor(readonly httpClient: HttpClient) {
    this.userinfo();
  }
  user: any = null;

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
      .subscribe((user) => {
        console.log(user);
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
