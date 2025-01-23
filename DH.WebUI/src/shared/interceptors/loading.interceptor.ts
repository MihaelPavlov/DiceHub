import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor,
} from '@angular/common/http';
import { Observable } from 'rxjs';
import { finalize } from 'rxjs/operators';
import { LoadingService } from '../services/loading.service';

@Injectable()
export class LoadingInterceptor implements HttpInterceptor {
  constructor(private loadingService: LoadingService) {}

  intercept(
    request: HttpRequest<any>,
    next: HttpHandler
  ): Observable<HttpEvent<any>> {
    const isBackgroundRequest = request.headers.get('X-Background-Request');

    if (isBackgroundRequest && isBackgroundRequest === 'true') {
      return next.handle(request);
    }

    this.loadingService.loadingOn();
    return next
      .handle(request)
      .pipe(finalize(() => this.loadingService.loadingOff()));
  }
}
