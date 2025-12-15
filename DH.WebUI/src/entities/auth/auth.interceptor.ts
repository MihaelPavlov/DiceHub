import {
  HttpErrorResponse,
  HttpEvent,
  HttpHandler,
  HttpInterceptor,
  HttpRequest,
} from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { catchError, EMPTY, Observable } from 'rxjs';
import { AuthTokenService } from '../../shared/services/auth-token.service';

@Injectable()
export class HttpRequestInterceptor implements HttpInterceptor {
  constructor(
    private tokenService: AuthTokenService,
    private router: Router,
    private translateService: TranslateService
  ) {}

  intercept(
    req: HttpRequest<any>,
    next: HttpHandler
  ): Observable<HttpEvent<any>> {
    let headers = req.headers;

    // Only set JSON if not FormData
    if (!(req.body instanceof FormData) && !headers.has('Content-Type')) {
      headers = headers.set('Content-Type', 'application/json');
    }

    // Add JWT token
    const token = this.tokenService.getToken();
    if (token && !headers.has('Authorization')) {
      headers = headers.set('Authorization', `Bearer ${token}`);
    }

    // Preserve Accept-Language
    if (!headers.has('Accept-Language')) {
      const lang = this.translateService.getCurrentLang();
      if (lang) headers = headers.set('Accept-Language', lang);
    }

    const clonedReq = req.clone({ headers });
    const isBackground = clonedReq.headers.has('X-Background-Request');

    return next.handle(clonedReq).pipe(
      catchError((error: HttpErrorResponse) => {
        if (error.status === 401) {
          if (isBackground) return EMPTY; // silent fail for background
          this.tokenService.clearToken();
          this.router.navigateByUrl('/login');
          return EMPTY;
        }
        throw error;
      })
    );
  }
}
