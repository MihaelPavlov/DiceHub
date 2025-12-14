import { LanguageService } from './../../shared/services/language.service';
import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, map, tap } from 'rxjs';
import { IUserInfo } from './models/user-info.model';
import { Router } from '@angular/router';
import { ITokenResponse } from './models/token-response.model';
import { RestApiService } from '../../shared/services/rest-api.service';
import { IRegisterRequest } from './models/register.model';
import { PATH } from '../../shared/configs/path.config';
import { IResetPasswordRequest } from './models/reset-password-request.model';
import { IRegisterResponse } from './models/register-response.model';
import { ICreateEmployeePasswordRequest } from './models/create-employee-password.model';
import { AppToastMessage } from '../../shared/components/toast/constants/app-toast-messages.constant';
import { TenantSettingsService } from '../common/api/tenant-settings.service';
import { FULL_ROUTE } from '../../shared/configs/route.config';
import { UserRole } from './enums/roles.enum';
import { ICreateOwnerPasswordRequest } from './models/create-owner-password.model';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  public readonly userInfoSubject$: BehaviorSubject<IUserInfo | null> =
    new BehaviorSubject<IUserInfo | null>(null);

  public userInfo$ = this.userInfoSubject$.asObservable();

  constructor(
    readonly api: RestApiService,
    private readonly router: Router,
    private readonly tenantSettingsService: TenantSettingsService,
    private readonly languageService: LanguageService
  ) {
    if (!this.userInfoSubject$.value) {
      this.userinfo();
    }
  }

  public confirmEmail(
    email: string,
    token: string,
    timeZone: string
  ): Observable<ITokenResponse | null> {
    return this.api.post<ITokenResponse>('/api/user/confirm-email', {
      email,
      token,
      timeZone,
    });
  }

  public forgotPassword(email: string, language: string): Observable<any> {
    return this.api.post(`/api/user/forgot-password/${email}/${language}`, {});
  }

  public sendEmailConfirmationRequest(
    email: string,
    language: string
  ): Observable<boolean | null> {
    return this.api.post<boolean>(
      `/api/user/send-email-confirmation-request/${email}/${language}`,
      {}
    );
  }

  public sendEmployeePasswordResetRequest(
    email: string
  ): Observable<boolean | null> {
    return this.api.post<boolean>(
      `/api/user/send-employee-password-reset-request/${email}`,
      {}
    );
  }

  public sendOwnerPasswordResetRequest(
    email: string
  ): Observable<boolean | null> {
    return this.api.post<boolean>(
      `/api/user/send-owner-password-reset-request/${email}`,
      {}
    );
  }

  public createEmployeePassword(
    request: ICreateEmployeePasswordRequest
  ): Observable<any> {
    return this.api.post(`/api/user/create-employee-password`, request);
  }

  public createOwnerPassword(
    request: ICreateOwnerPasswordRequest
  ): Observable<any> {
    return this.api.post(`/api/user/create-owner-password`, request);
  }

  public resetPassword(request: IResetPasswordRequest): Observable<any> {
    return this.api.post(`/api/user/reset-password`, request);
  }

  public get getUser(): IUserInfo | null {
    return this.userInfoSubject$.value;
  }

  public login(loginForm: any): Observable<ITokenResponse | null> {
    return this.api.post<ITokenResponse>('/api/user', loginForm);
  }

  public authenticateUser(accessToken: string, refreshToken: string): void {
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

  public register(
    registerForm: IRegisterRequest
  ): Observable<IRegisterResponse | null> {
    return this.api.post<IRegisterResponse>(
      '/api/user/register-user',
      registerForm
    );
  }

  public registerNotification(email: any): Observable<null> {
    return this.api.post<any>('/api/user/register-notification', { email });
  }

  public userinfo(): void {
    const sidClaim: string =
      'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/sid';
    const roleNameClaim: string =
      'http://schemas.microsoft.com/ws/2008/06/identity/claims/role';
    const roleClaim: string = 'role_key';

    const usernameClaim: string =
      'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name';

    this.api.get('/api/user/info').subscribe({
      next: (user: any) => {
        if (user) {
          this.userInfoSubject$.next({
            id: user[sidClaim],
            role: user[roleClaim],
            username: user[usernameClaim],
            permissionString: user['permissions'],
          });

          this.languageService.loadUserLanguage();

          this.tenantSettingsService.get().subscribe({
            next: (settings) => {
              if (
                settings &&
                settings.isCustomPeriodOn &&
                !settings.isCustomPeriodSetupComplete &&
                this.userInfoSubject$.value?.role != UserRole.User
              )
                this.router.navigateByUrl(
                  FULL_ROUTE.CHALLENGES.ADMIN_CUSTOM_PERIOD
                );
            },
          });
        } else {
          this.userInfoSubject$.next(null);
        }
      },
      error: () => {
        console.error(AppToastMessage.SomethingWrong);
        this.userInfoSubject$.next(null);
      },
    });
  }

  public userinfo$(): Promise<void> {
    const sidClaim: string =
      'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/sid';
    const roleNameClaim: string =
      'http://schemas.microsoft.com/ws/2008/06/identity/claims/role';
    const roleClaim: string = 'role_key';
    const usernameClaim: string =
      'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name';

    return new Promise((resolve) => {
      this.api.get('/api/user/info').subscribe({
        next: (user: any) => {
          if (user) {
            this.userInfoSubject$.next({
              id: user[sidClaim],
              role: user[roleClaim],
              username: user[usernameClaim],
              permissionString: user['permissions'],
            });

            this.languageService.loadUserLanguage();

            this.tenantSettingsService.get().subscribe({
              next: (settings) => {
                if (
                  settings &&
                  settings.isCustomPeriodOn &&
                  !settings.isCustomPeriodSetupComplete &&
                  this.userInfoSubject$.value?.role != UserRole.User
                )
                  this.router.navigateByUrl(
                    FULL_ROUTE.CHALLENGES.ADMIN_CUSTOM_PERIOD
                  );

                resolve();
              },
            });
          } else {
            this.userInfoSubject$.next(null);
            console.warn('User is not sign in');
            resolve();
          }
        },
        error: (error) => {
          this.userInfoSubject$.next(null);
          resolve();
        },
      });
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

  public logout(): Observable<void | null> {
    return this.api
      .post<void>(`/${PATH.USER.CORE}/${PATH.USER.LOGOUT}`, {})
      .pipe(
        tap(() => {
          localStorage.removeItem('jwt');
          localStorage.removeItem('refreshToken');
          this.userInfoSubject$.next(null);
        })
      );
  }

  public saveToken(deviceToken: string): Observable<null> {
    return this.api.post(`/${PATH.USER.CORE}/${PATH.USER.SAVE_TOKEN}`, {
      deviceToken,
    });
  }
}
