import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class MenuTabsService {
  private activeTabSubject = new BehaviorSubject<string>('');
  public activeTab$ = this.activeTabSubject.asObservable();

  public setActive(label: string): void {
    this.activeTabSubject.next(label);
  }

  public resetData(): void {
    this.activeTabSubject.next('');
  }
}
