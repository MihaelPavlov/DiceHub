import { GlobalErrorHandler } from './../shared/components/global-error-handler';
import { ConfirmEmailModule } from './../pages/confirm-email/confirm-email.module';
import {
  NgModule,
  ErrorHandler,
  APP_INITIALIZER,
  LOCALE_ID,
} from '@angular/core';
import { AppComponent } from './app-component/app.component';
import { HTTP_INTERCEPTORS, HttpClientModule } from '@angular/common/http';
import { BrowserModule } from '@angular/platform-browser';
import { NavigationMenuModule } from '../widgets/menu/navigation-menu.module';
import { RouterOutlet } from '@angular/router';
import { AppRoutingModule } from './app-routes/app-routes.module';
import { HeaderModule } from '../widgets/header/header.module';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { LoginModule } from '../pages/login/login.module';
import { AuthGuard } from '../shared/guards/auth.guard';
import { JWT_OPTIONS, JwtHelperService, JwtModule } from '@auth0/angular-jwt';
import { HttpRequestInterceptor } from '../entities/auth/auth.interceptor';
import { ErrorInterceptor } from '../shared/interceptors/http-error.interceptor';
import { LoadingIndicatorComponent } from '../shared/components/loading-indicator/loading-indicator.component';
import { FirebaseModule } from '../shared/firebase.module';
import { RegisterModule } from '../pages/register/register.module';
import { AssistiveTouchModule } from '../shared/components/assistive-touch/assistive-touch.module';
import { LoadingInterceptor } from '../shared/interceptors/loading.interceptor';
import { ScrollToTopModule } from '../shared/components/scroll-to-top/scroll-to-top.module';
import { ForgotPasswordModule } from '../pages/forgot-password/forgot-password.module';
import { ResetPasswordModule } from '../pages/reset-password/reset-password.module';
import { AuthRedirectGuard } from '../shared/guards/auth-redirect.guard';
import { CreateEmployeePasswordModule } from '../pages/create-employee-password/create-employee-password.module';
import { CreateOwnerPasswordModule } from '../pages/create-owner-password/create-owner-password.module';
import { AuthService } from '../entities/auth/auth.service';
import { TranslateModule } from '@ngx-translate/core';
import { provideTranslateHttpLoader } from '@ngx-translate/http-loader';
import { registerLocaleData } from '@angular/common';
import localeBg from '@angular/common/locales/bg';
import localeEn from '@angular/common/locales/en';

export function initializeUserFactory(authService: AuthService): () => void {
  return () => authService.userinfo$();
}
registerLocaleData(localeBg, 'bg');
registerLocaleData(localeEn, 'en');

@NgModule({
  declarations: [AppComponent],
  exports: [BrowserModule, BrowserAnimationsModule],
  providers: [
    AuthGuard,
    AuthRedirectGuard,
    {
      provide: APP_INITIALIZER,
      useFactory: initializeUserFactory,
      deps: [AuthService],
      multi: true,
    },
    ...provideTranslateHttpLoader({
      prefix: 'shared/assets/i18n/',
      suffix: '.json',
    }),
    { provide: JWT_OPTIONS, useValue: JWT_OPTIONS },
    { provide: HTTP_INTERCEPTORS, useClass: ErrorInterceptor, multi: true },
    {
      provide: HTTP_INTERCEPTORS,
      useClass: HttpRequestInterceptor,
      multi: true,
    },
    {
      provide: HTTP_INTERCEPTORS,
      useClass: LoadingInterceptor,
      multi: true,
    },
    {
      provide: ErrorHandler,
      useClass: GlobalErrorHandler,
    },
    JwtHelperService,
  ],
  bootstrap: [AppComponent],
  imports: [
    TranslateModule.forRoot(),
    HttpClientModule,
    AppRoutingModule,
    BrowserModule,
    NavigationMenuModule,
    LoginModule,
    RegisterModule,
    ConfirmEmailModule,
    ForgotPasswordModule,
    ResetPasswordModule,
    CreateEmployeePasswordModule,
    CreateOwnerPasswordModule,
    HeaderModule,
    AssistiveTouchModule,
    RouterOutlet,
    JwtModule,
    LoadingIndicatorComponent,
    FirebaseModule,
    ScrollToTopModule,
  ],
})
export class AppModule {}
