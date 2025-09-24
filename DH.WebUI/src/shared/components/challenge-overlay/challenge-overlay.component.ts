import { Component, Input } from '@angular/core';
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
  @Input() message: string = '🎉 Challenge Completed!';
  @Input() reward: string = '+50 XP';
  @Input() percent: number = 0;
  // Completion overlay
  showCompletion = false;
  completionMessage = '';
  completionReward = '';

  // Progress overlay
  showProgress = false;
  progressMessage = '';

  // ------------------------
  // Trigger completion
  // ------------------------
  completeChallenge(message: string, reward: string) {
    this.completionMessage = message;
    this.completionReward = reward;
    this.showCompletion = true;
  }

  // ------------------------
  // Trigger progress
  // ------------------------
  updateProgress(progressPercent: number) {
    this.progressMessage = `You reached ${progressPercent}% of the challenge!`;
    this.showProgress = true;
  }

  // ------------------------
  // Close handlers
  // ------------------------
  closeCompletion() {
    this.showCompletion = false;
  }

  closeProgress() {
    this.showProgress = false;
  }
}
