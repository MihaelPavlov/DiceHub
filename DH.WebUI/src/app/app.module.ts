import { NgModule } from '@angular/core';
import { AppComponent } from './app-component/app.component';
import { HTTP_INTERCEPTORS, HttpClientModule } from '@angular/common/http';
import { BrowserModule } from '@angular/platform-browser';
import { MenuModule } from '../widgets/menu/menu.module';
import { RouterOutlet } from '@angular/router';
import { AppRoutingModule } from './app-routes/app-routes.module';
import { HeaderModule } from '../widgets/header/header.module';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { LoginModule } from '../pages/login/login.module';
import { AuthGuard } from '../shared/guards/auth.guard';
import { JWT_OPTIONS, JwtHelperService, JwtModule } from '@auth0/angular-jwt';
import { HttpRequestInterceptor } from '../entities/auth/auth.interceptor';
import { ErrorInterceptor } from '../shared/interceptors/http-error.interceptor';
import { LoadingIndicatorComponent } from "../shared/components/loading-indicator/loading-indicator.component";

@NgModule({
  declarations: [AppComponent],
  exports: [BrowserModule, BrowserAnimationsModule],
  providers: [
    AuthGuard,
    { provide: JWT_OPTIONS, useValue: JWT_OPTIONS },
    { provide: HTTP_INTERCEPTORS, useClass: ErrorInterceptor, multi: true },
    {
      provide: HTTP_INTERCEPTORS,
      useClass: HttpRequestInterceptor,
      multi: true,
    },
    JwtHelperService,
  ],
  bootstrap: [AppComponent],
  imports: [
    HttpClientModule,
    AppRoutingModule,
    BrowserModule,
    MenuModule,
    LoginModule,
    HeaderModule,
    RouterOutlet,
    JwtModule,
    LoadingIndicatorComponent
],
})
export class AppModule {}
