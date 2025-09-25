import { Component } from '@angular/core';
import {
  trigger,
  transition,
  style,
  animate,
  keyframes,
} from '@angular/animations';

@Component({
  selector: 'app-challenge-overlay',
  templateUrl: './challenge-overlay.component.html',
  styleUrls: ['./challenge-overlay.component.scss'],
  animations: [
    trigger('popIn', [
      transition(':enter', [
        animate(
          '700ms ease-out',
          keyframes([
            style({ transform: 'scale(0)', opacity: 0, offset: 0 }),
            style({ transform: 'scale(1.2)', opacity: 1, offset: 0.6 }),
            style({ transform: 'scale(1)', opacity: 1, offset: 1 }),
          ])
        ),
      ]),
      transition(':leave', [
        animate(
          '500ms ease-in',
          keyframes([
            style({ transform: 'scale(1)', opacity: 1, offset: 0 }),
            style({ transform: 'scale(0.7)', opacity: 0, offset: 1 }),
          ])
        ),
      ]),
    ]),
  ],
})
export class ChallengeOverlayComponent {
  // Completion overlay
  public showCompletion = false;
  public completionMessage = '';
  public completionReward: string = '';

  // Progress overlay
  public showProgress = false;
  public progressMessage = '';

  public completeChallenge(challengeGameName: string, reward: number): void {
    this.completionMessage = `Challenge for Game ${challengeGameName} is completed!`;
    this.completionReward = `+ ${reward} Points`;
    this.showCompletion = true;
  }

  public updateProgress(challengeGameName: string): void {
    this.progressMessage = `Progress on challenge for game ${challengeGameName} has been updated. <br/>Keep going to complete the challenge!`;
    this.showProgress = true;
  }

  public closeCompletion(): void {
    this.showCompletion = false;
  }

  public closeProgress(): void {
    this.showProgress = false;
  }
}
