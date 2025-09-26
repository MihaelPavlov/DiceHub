import { Component, ViewEncapsulation } from '@angular/core';
import {
  trigger,
  transition,
  style,
  animate,
  keyframes,
} from '@angular/animations';
import { DomSanitizer, SafeHtml } from '@angular/platform-browser';
import { TranslateService } from '@ngx-translate/core';
import { LanguageService } from '../../services/language.service';
import { SupportLanguages } from '../../../entities/common/models/support-languages.enum';

@Component({
  selector: 'app-challenge-overlay',
  templateUrl: './challenge-overlay.component.html',
  styleUrls: ['./challenge-overlay.component.scss'],
  encapsulation: ViewEncapsulation.None,
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
  public showChallengeCompletion: boolean = false;
  public challengeCompletionMessage!: SafeHtml;
  public challengeCompletionReward: string = '';

  // Progress overlay
  public showChallengeProgress: boolean = false;
  public challengeProgressMessage!: SafeHtml;

  public showRewardGranted: boolean = false;
  public rewardGrantedMessage!: SafeHtml;

  constructor(
    private readonly ts: TranslateService,
    private readonly languageService: LanguageService,
    private readonly sanitizer: DomSanitizer
  ) {}

  public completeChallenge(challengeGameName: string, reward: number): void {
    const message = this.ts.instant('challenge_overlay.completed_challenge', {
      challengeGameName,
    });
    this.challengeCompletionMessage =
      this.sanitizer.bypassSecurityTrustHtml(message);

    this.challengeCompletionReward = this.ts.instant(
      'challenge_overlay.reward_points',
      {
        reward,
      }
    );
    this.showChallengeCompletion = true;
  }

  public updateProgress(challengeGameName: string): void {
    const message = this.ts.instant('challenge_overlay.updated_challenge', {
      challengeGameName,
    });
    this.challengeProgressMessage =
      this.sanitizer.bypassSecurityTrustHtml(message);
    this.showChallengeProgress = true;
  }

  public rewardGranted(name_bg: string, name_en: string): void {
    const name =
      this.languageService.getCurrentLanguage() === SupportLanguages.EN
        ? name_en
        : name_bg;

    const message = this.ts.instant('challenge_overlay.reward_granted', {
      name,
    });
    this.rewardGrantedMessage = this.sanitizer.bypassSecurityTrustHtml(message);

    this.showRewardGranted = true;
  }

  public closeChallengeCompletion(): void {
    this.showChallengeCompletion = false;
  }

  public closeChallengeProgress(): void {
    this.showChallengeProgress = false;
  }

  public closeRewardGranted(): void {
    this.showRewardGranted = false;
  }
}
