import {
  HttpEvent,
  HttpHandler,
  HttpInterceptor,
  HttpRequest,
} from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

@Injectable()
export class HttpRequestInterceptor implements HttpInterceptor {
  intercept(
    req: HttpRequest<any>,
    next: HttpHandler
  ): Observable<HttpEvent<any>> {
    const token = localStorage.getItem('jwt');

    let headers = req.headers
    if (!(req.body instanceof FormData)) {
      headers = headers.set('Content-Type', 'application/json');
    }
    if (token) {
      headers = headers.set('Authorization', `Bearer ${token}`);
    }

    req = req.clone({ headers });

    return next.handle(req);
  }
}
