import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class NavigationService {
  private previousUrl: string = '';

  // Set the previous URL manually
  setPreviousUrl(url: string): void {
    this.previousUrl = url;
  }

  // Get the previous URL
  getPreviousUrl(): string {
    return this.previousUrl;
  }
}
