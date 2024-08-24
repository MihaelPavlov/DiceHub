import { Injectable } from '@angular/core';
import {
  HttpEvent,
  HttpInterceptor,
  HttpHandler,
  HttpRequest,
  HttpErrorResponse,
} from '@angular/common/http';
import { Observable, of, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { Router } from '@angular/router';

@Injectable()
export class ErrorInterceptor implements HttpInterceptor {
  constructor(private router: Router) {}

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    return next.handle(req).pipe(
      catchError((error: HttpErrorResponse) => {
        let errorMessage = 'An unknown error occurred!';

        if (error.error instanceof ErrorEvent) {
          return of();
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

            default:
              return of();
          }
        }

        // Optionally, you can log the error or display a notification
        console.error(errorMessage);

        // Return the error message as an observable
        return throwError(() => new Error(errorMessage));
      })
    );
  }
}