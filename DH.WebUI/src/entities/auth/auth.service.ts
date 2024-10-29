import { HttpClient, HttpHeaders } from '@angular/common/http';
import { ChangeDetectorRef, Injectable } from '@angular/core';
import { BehaviorSubject, Observable, map } from 'rxjs';
import { IUserInfo } from './models/user-info.model';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private readonly userInfoSubject$: BehaviorSubject<IUserInfo | null> =
    new BehaviorSubject<IUserInfo | null>(null);

  public userInfo$ = this.userInfoSubject$.asObservable();

  constructor(
    readonly httpClient: HttpClient,
    private readonly router: Router
  ) {}

  public get getUser(): IUserInfo | null {
    return this.userInfoSubject$.value;
  }

  login(loginForm: any, withRegisterNotification: boolean = false) {
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
        this.router.navigateByUrl('games/library');
        if (withRegisterNotification) {
          this.registerNotification(loginForm.email).subscribe();
        }
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

  register(registerForm: any): Observable<null> {
    return this.httpClient.post<any>(
      'https://localhost:7024/user/register',
      registerForm
    );
  }

  registerNotification(email: any): Observable<null> {
    return this.httpClient.post<any>(
      'https://localhost:7024/user/register-notification',
      { email }
    );
  }

  //TODO: Change it to private in future
  public userinfo(): void {
    const sidClaim: string =
      'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/sid';
    const roleClaim: string =
      'http://schemas.microsoft.com/ws/2008/06/identity/claims/role';

    this.httpClient.get('https://localhost:7024/user/info').subscribe({
      next: (user: any) => {
        if (user)
          this.userInfoSubject$.next({
            id: user[sidClaim],
            role: user[roleClaim],
            permissionString: user['permissions'],
          });
        else {
          this.userInfoSubject$.next(null);
        }
        console.log(this.userInfoSubject$.value);
      },
      error: () => {
        this.userInfoSubject$.next(null);
        this.router.navigateByUrl("login")
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

  logout() {
    localStorage.removeItem('jwt');
    localStorage.removeItem('refreshToken');
    this.userInfoSubject$.next(null);
  }

  removeUserInfo() {
    this.userInfoSubject$.next(null);
  }
}

export interface AuthenticatedResponse {
  accessToken: string;
  refreshToken: string;
}
