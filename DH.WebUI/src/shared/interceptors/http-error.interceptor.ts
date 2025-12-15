import { Injectable } from '@angular/core';
import { HttpEvent, HttpInterceptor, HttpHandler, HttpRequest, HttpErrorResponse } from '@angular/common/http';
import { EMPTY, Observable, of, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { Router } from '@angular/router';

@Injectable()
export class ErrorInterceptor implements HttpInterceptor {
  constructor(private router: Router) {}

  intercept(
    req: HttpRequest<any>,
    next: HttpHandler
  ): Observable<HttpEvent<any>> {
    const isBackground = req.headers.has('X-Background-Request');

    return next.handle(req).pipe(
      catchError((error: HttpErrorResponse) => {
        if (isBackground) {
          // Fail silently for background requests
          return EMPTY;
        }
        let errorMessage = 'An unknown error occurred!';

        if (error.error instanceof ErrorEvent) {
          return throwError(() => error);
        } else {
          // Server-side error          
          switch (error.status) {
            case 400:
              errorMessage = 'Bad Request: ' + error.error.detail;
              break;
            case 401:
              this.router.navigate(['/unauthorized']);
              break;
            case 403:
              this.router.navigate(['/forbidden']);
              break;
            case 404:
              this.router.navigate(['/not-found']);
              break;
            case 0:
              this.router.navigate(['/server-error']);
              break;
          }
        }

        // Return the error message as an observable
        return throwError(() => error);
      })
    );
  }
}
