import { FrontEndLogService } from '../services/frontend-log.service';
import { ErrorHandler, Injectable } from '@angular/core';

@Injectable()
export class GlobalErrorHandler implements ErrorHandler {
  constructor(private readonly frontEndLogService: FrontEndLogService) {}
  handleError(error: any): void {
    this.frontEndLogService.sendWarning(error.message, error.stack).subscribe({
      next: (response) => {
        console.log('Error logged successfully:', response);
      },
    });
  }
}
