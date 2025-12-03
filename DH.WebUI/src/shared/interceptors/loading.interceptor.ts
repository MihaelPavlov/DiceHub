import { Injectable } from '@angular/core';
import { HttpRequest, HttpHandler, HttpEvent, HttpInterceptor } from '@angular/common/http';
import { Observable } from 'rxjs';
import { finalize } from 'rxjs/operators';
import { LoadingService } from '../services/loading.service';
import { LoadingInterceptorContextService } from '../services/loading-context.service';

@Injectable()
export class LoadingInterceptor implements HttpInterceptor {
  constructor(
    private loadingService: LoadingService,
    private context: LoadingInterceptorContextService
  ) {}

  intercept(
    request: HttpRequest<any>,
    next: HttpHandler
  ): Observable<HttpEvent<any>> {
    const isBackground = request.headers.get('X-Background-Request') === 'true';

    if (this.context.isManualMode() || isBackground) {
      return next.handle(request);
    }

    this.loadingService.loadingOn();
    return next
      .handle(request)
      .pipe(finalize(() => this.loadingService.loadingOff()));
  }
}
