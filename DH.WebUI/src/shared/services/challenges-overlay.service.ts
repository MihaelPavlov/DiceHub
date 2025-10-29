import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { ChallengeOverlayComponent } from '../components/challenge-overlay/challenge-overlay.component';

@Injectable({ providedIn: 'root' })
export class ChallengeOverlayService {
  public overlay = new BehaviorSubject<ChallengeOverlayComponent | null>(null);

  public init(challengeOverlayComponent: ChallengeOverlayComponent): void {
    this.overlay.next(challengeOverlayComponent);
  }
}
