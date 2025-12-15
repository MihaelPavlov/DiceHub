import { FrontEndLogService } from '../services/frontend-log.service';
import { ErrorHandler, Injectable } from '@angular/core';

@Injectable()
export class GlobalErrorHandler implements ErrorHandler {
  constructor(private readonly frontEndLogService: FrontEndLogService) {}
  handleError(error: any): void {
   // Check if error originated from a background request
    const isBackgroundRequest = error?.url && error?.headers?.has?.('X-Background-Request');
    
    if (isBackgroundRequest) {
      // Skip logging for background requests
      return;
    }

    // Send other errors to backend logging
    this.frontEndLogService.sendWarning(error.message, error.stack ?? '').subscribe({
      next: () => {},
      error: () => {
        // Avoid throwing errors in the error handler itself
      },
    });
  }
}
