import { Injectable } from '@angular/core';
import { HttpRequest, HttpHandler, HttpEvent, HttpInterceptor } from '@angular/common/http';
import { Observable } from 'rxjs';
import { finalize } from 'rxjs/operators';
import { LoadingService } from '../services/loading.service';
import { LoadingInterceptorContextService } from '../services/loading-context.service';

@Injectable()
export class LoadingInterceptor implements HttpInterceptor {
  // Registered once at the root, so this counter is shared across every
  // intercepted request. Without it, firing requests in parallel (e.g. a
  // combineLatest/forkJoin save) would hide the spinner the moment the
  // *fastest* one finished, while the slow request was still running.
  private activeRequests = 0;

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

    if (this.activeRequests === 0) {
      this.loadingService.loadingOn();
    }
    this.activeRequests++;

    return next.handle(request).pipe(
      finalize(() => {
        this.activeRequests = Math.max(0, this.activeRequests - 1);
        if (this.activeRequests === 0) {
          this.loadingService.loadingOff();
        }
      })
    );
  }
}
