import { Injectable } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class LoadingInterceptorContextService {
  private manualMode = false;

  public enableManualMode(): void {
    this.manualMode = true;
  }

  public disableManualMode(): void {
    this.manualMode = false;
  }

  public isManualMode(): boolean {
    return this.manualMode;
  }
}
