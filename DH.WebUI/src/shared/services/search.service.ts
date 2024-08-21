import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class SearchService {
  private searchFormVisibleSubject = new BehaviorSubject<boolean>(false);
  public searchFormVisible$ = this.searchFormVisibleSubject.asObservable();

  constructor() {}

  public showSearchForm() {
    this.searchFormVisibleSubject.next(true);
  }

  public hideSearchForm() {
    this.searchFormVisibleSubject.next(false);
  }

  public toggleSearchForm() {
    this.searchFormVisibleSubject.next(!this.searchFormVisibleSubject.value);
  }
}
