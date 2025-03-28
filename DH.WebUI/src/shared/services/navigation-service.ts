import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class NavigationService {
  private previousUrl: string | null = null;

  // Set the previous URL manually
  setPreviousUrl(url: string): void {
    this.previousUrl = url;
  }

  // Get the previous URL
  getPreviousUrl(): string | null {
    return this.previousUrl;
  }
}
