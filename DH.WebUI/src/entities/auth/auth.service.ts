import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, map } from 'rxjs';
import { IUserInfo } from './models/user-info.model';
import { Router } from '@angular/router';
import { ITokenResponse } from './models/token-response.model';
import { RestApiService } from '../../shared/services/rest-api.service';
import { IRegisterRequest } from './models/register.model';
import { PATH } from '../../shared/configs/path.config';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private readonly userInfoSubject$: BehaviorSubject<IUserInfo | null> =
    new BehaviorSubject<IUserInfo | null>(null);

  public userInfo$ = this.userInfoSubject$.asObservable();

  constructor(readonly api: RestApiService, private readonly router: Router) {}

  public get getUser(): IUserInfo | null {
    return this.userInfoSubject$.value;
  }

  public login(loginForm: any): Observable<ITokenResponse | null> {
    return this.api.post<ITokenResponse>('/user', loginForm);
  }

  public onnSuccessfullyLogin(accessToken: string, refreshToken: string): void {
    localStorage.setItem('jwt', accessToken);
    localStorage.setItem('refreshToken', refreshToken);
    this.userinfo();
  }

  public initiateNotifications(email: string): void {
    this.registerNotification(email).subscribe({
      next: () => {
        console.log('success send register not');
      },
      error: (error) => {
        console.log('error notification not send', error);
      },
    });
  }

  public register(registerForm: IRegisterRequest): Observable<null> {
    return this.api.post('/user/register-user', registerForm);
  }

  // For tests
  game(gameForm: any) {
    return this.api.post<any>('/games', gameForm).subscribe((response) => {
      console.log(response);
    });
  }

  public registerNotification(email: any): Observable<null> {
    return this.api.post<any>('/user/register-notification', { email });
  }

  public userinfo(): void {
    const sidClaim: string =
      'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/sid';
    const roleClaim: string =
      'http://schemas.microsoft.com/ws/2008/06/identity/claims/role';
    const usernameClaim: string =
      'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name';

    this.api.get('/user/info').subscribe({
      next: (user: any) => {
        if (user)
          this.userInfoSubject$.next({
            id: user[sidClaim],
            role: user[roleClaim],
            username: user[usernameClaim],
            permissionString: user['permissions'],
          });
        else {
          this.userInfoSubject$.next(null);
        }
        console.log(this.userInfoSubject$.value);
      },
      error: () => {
        this.userInfoSubject$.next(null);
        //TODO: This need to be removed, because it's call on app.component.ts and every time 
        // when we try to reach different page from register and login we will be redirected to login
        // this.router.navigateByUrl('login');
      },
    });
  }

  public isAuthenticated(): Observable<boolean> {
    return this.userInfo$.pipe(
      map((user) => {
        return user ? true : false;
      })
    );
  }
  public getToken(): string | null {
    return localStorage.getItem('jwt');
  }
  public logout(): void {
    localStorage.removeItem('jwt');
    localStorage.removeItem('refreshToken');
    this.userInfoSubject$.next(null);
  }

  public saveToken(deviceToken: string): Observable<null> {
    return this.api.post(`/${PATH.USER.CORE}/${PATH.USER.SAVE_TOKEN}`, {
      deviceToken,
    });
  }
}
