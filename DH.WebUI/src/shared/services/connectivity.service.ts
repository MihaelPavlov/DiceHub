import { Injectable } from '@angular/core';
import { Observable, fromEvent, merge } from 'rxjs';
import { map, shareReplay, startWith } from 'rxjs/operators';

@Injectable({ providedIn: 'root' })
export class ConnectivityService {
  public readonly isOnline$: Observable<boolean> = merge(
    fromEvent(window, 'online').pipe(map(() => true)),
    fromEvent(window, 'offline').pipe(map(() => false))
  ).pipe(
    startWith(navigator.onLine),
    shareReplay({ bufferSize: 1, refCount: false })
  );

  public get isOnline(): boolean {
    return navigator.onLine;
  }
}
