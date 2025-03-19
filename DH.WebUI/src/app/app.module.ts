import { InstructionManagementModule } from './../pages/instruction-management/instruction-management.module';
import { NgModule } from '@angular/core';
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
    {
      provide: HTTP_INTERCEPTORS,
      useClass: LoadingInterceptor,
      multi: true,
    },
    JwtHelperService,
  ],
  bootstrap: [AppComponent],
  imports: [
    HttpClientModule,
    AppRoutingModule,
    BrowserModule,
    NavigationMenuModule,
    LoginModule,
    RegisterModule,
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
