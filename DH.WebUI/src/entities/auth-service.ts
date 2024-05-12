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
        const token = response.token;
        localStorage.setItem('jwt', token);
        this.userinfo();
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
      .post<any>('https://localhost:7024/game', loginForm, httpsOptions)
      .subscribe((response) => {
        
      });
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
  }
}
