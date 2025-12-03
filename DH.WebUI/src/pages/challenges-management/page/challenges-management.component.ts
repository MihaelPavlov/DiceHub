import { TenantSettingsService } from './../../../entities/common/api/tenant-settings.service';
import { DateHelper } from './../../../shared/helpers/date-helper';
import { IUserChallengePeriodReward } from './../../../entities/rewards/models/user-period-reward.model';
import { IUserChallenge } from './../../../entities/challenges/models/user-challenge.model';
import {
  Component,
  OnInit,
  ChangeDetectorRef,
  ViewChild,
  ElementRef,
  ViewChildren,
  QueryList,
  Renderer2,
  OnDestroy,
} from '@angular/core';
import { ChallengesService } from '../../../entities/challenges/api/challenges.service';
import { RewardsService } from '../../../entities/rewards/api/rewards.service';
import { IUserChallengePeriodPerformance } from '../../../entities/challenges/models/user-challenge-period-performance.model';
import { combineLatest, Subscription, timer } from 'rxjs';
import { ChallengeStatus } from '../../../entities/challenges/enums/challenge-status.enum';
import { Column } from '../../../widgets/nav-bar/page/nav-bar.component';
import { FULL_ROUTE } from '../../../shared/configs/route.config';
import { ImageEntityType } from '../../../shared/pipe/entity-image.pipe';
import { ITenantSettings } from '../../../entities/common/models/tenant-settings.model';
import {
  IUserCustomPeriodChallenge,
  IUserCustomPeriodReward,
} from '../../../entities/challenges/models/user-custom-period.model';
import {
  ImagePreviewDialog,
  ImagePreviewData,
} from '../../../shared/dialogs/image-preview/image-preview.dialog';
import { MatDialog } from '@angular/material/dialog';
import { LoadingInterceptorContextService } from '../../../shared/services/loading-context.service';
import { LoadingService } from '../../../shared/services/loading.service';
import { TranslateService } from '@ngx-translate/core';
import { SupportLanguages } from '../../../entities/common/models/support-languages.enum';
import { ChallengeType } from '../shared/challenge-type.enum';
import { UniversalChallengeType } from '../shared/challenge-universal-type.enum';
import { LanguageService } from '../../../shared/services/language.service';
import { IUserUniversalChallenge } from '../../../entities/challenges/models/user-universal-challenge.model';
import { ControlsMenuComponent } from '../../../shared/components/menu/controls-menu.component';
import { QrCodeDialog } from '../../../features/games-library/dialogs/qr-code-dialog/qr-code-dialog.component';
import { QrCodeType } from '../../../entities/qr-code-scanner/enums/qr-code-type.enum';
import { AuthService } from '../../../entities/auth/auth.service';

@Component({
    selector: 'app-challenges-management',
    templateUrl: 'challenges-management.component.html',
    styleUrl: 'challenges-management.component.scss',
    standalone: false
})
export class ChallengesManagementComponent implements OnInit, OnDestroy {
  @ViewChild('rewardsScroller') rewardsScroller!: ElementRef;
  @ViewChild('pointsScroller') pointsScroller!: ElementRef;

  @ViewChild('progressContainer') progressContainer!: ElementRef;
  @ViewChildren('circleContainer') circles!: QueryList<any>;

  private readonly scrollArrowWidth = 2.5;
  public rewardScrollArrowsOpacity: number = 1;
  public rewardScrollArrowsWidth: number = this.scrollArrowWidth;
  public pointsScrollArrowsOpacity: number = 1;
  public pointsScrollArrowsWidth: number = this.scrollArrowWidth;
  public periodPerformance!: IUserChallengePeriodPerformance | null;
  public tenantSettings!: ITenantSettings | null;
  public userChallengePeriodRewardList!: IUserChallengePeriodReward[];
  public userChallengeList: IUserChallenge[] = [];

  public userCustomPeriodChallengesList: IUserCustomPeriodChallenge[] = [];
  public userCustomPeriodRewardsList: IUserCustomPeriodReward[] = [];

  public userUniversalChallengeList: IUserUniversalChallenge[] = [];

  public ChallengeStatus = ChallengeStatus;
  public readonly ImageEntityType = ImageEntityType;
  public readonly DATE_FORMAT = DateHelper.DATE_FORMAT;

  public columns: Column[] = [
    // FUTURE TODO: Update the word weekly based on the settings. Weekly rewards could be monthly
    {
      name: this.translateService.instant(
        'challenge_management.columns.weekly'
      ),
      link: FULL_ROUTE.CHALLENGES.CHALLENGES_HOME,
      isActive: true,
    },
    {
      name: this.translateService.instant(
        'challenge_management.columns.rewards'
      ),
      link: FULL_ROUTE.CHALLENGES.CHALLENGES_REWARDS,
      isActive: false,
    },
    // Future Feature: Streak Page
    // {
    //   name: this.translateService.instant(
    //     'challenge_management.columns.streaks'
    //   ),
    //   link: FULL_ROUTE.CHALLENGES.CHALLENGES_STREAKS,
    //   isActive: false,
    // },
  ];

  private inactivityRewardsTimer!: Subscription;
  private inactivityPointsTimer!: Subscription;
  private rewardsScrolling = false;
  private pointsScrolling = false;
  private rewardsListener!: () => void;
  private pointsListener!: () => void;

  public readonly ChallengeType = ChallengeType;
  public currentActiveTab: ChallengeType = ChallengeType.Game;
  public UniversalChallengeType = UniversalChallengeType;
  public SupportLanguages = SupportLanguages;
 
  public animateChallengeProgress = false;
  shakeStates: boolean[] = [];
  private intervals: any[] = [];

  constructor(
    private readonly authService: AuthService,
    private readonly rewardsService: RewardsService,
    private readonly challengeService: ChallengesService,
    private readonly tenantSettingsService: TenantSettingsService,
    private readonly cd: ChangeDetectorRef,
    private renderer: Renderer2,
    private readonly dialog: MatDialog,
    private readonly loadingContext: LoadingInterceptorContextService,
    private readonly loadingService: LoadingService,
    private readonly translateService: TranslateService,
    public readonly languageService: LanguageService
  ) {}

  public ngOnDestroy(): void {
    if (this.rewardsListener) this.rewardsListener();
    if (this.pointsListener) this.pointsListener();

    this.intervals.forEach((i) => clearInterval(i));
  }

  public getCurrentLanguage(): SupportLanguages {
    return this.languageService.getCurrentLanguage();
  }

  public toggleMenu(menu: ControlsMenuComponent, description: string): void {
    menu.infoDescription = description;
    menu.toggleMenu();
  }

  public tabChange(type: ChallengeType): void {
    this.currentActiveTab = type;
    this.animateChallengeProgress = false;
    this.runChallengeAnimation();
  }

  public challengeProgress(
    challenge: IUserUniversalChallenge | IUserChallenge
  ): number {
    if (!challenge || !challenge.maxAttempts || challenge.maxAttempts === 0)
      return 0;
    return (challenge.currentAttempts / challenge.maxAttempts) * 100;
  }

  public customPeriodChallengeProgress(
    challenge: IUserCustomPeriodChallenge
  ): number {
    if (
      !challenge ||
      !challenge.challengeAttempts ||
      challenge.challengeAttempts === 0
    )
      return 0;
    return (challenge.currentAttempts / challenge.challengeAttempts) * 100;
  }

  public openImagePreview(imageUrl: string) {
    this.dialog.open<ImagePreviewDialog, ImagePreviewData>(ImagePreviewDialog, {
      data: {
        imageUrl,
        title: this.translateService.instant('image'),
      },
      width: '17rem',
    });
  }

  public ngOnInit(): void {
    this.shakeStates = new Array(this.userUniversalChallengeList.length).fill(
      false
    );
    this.initUserUniversalChallengesAnimation();
    this.loadingContext.enableManualMode();
    this.loadingService.loadingOn();
    combineLatest([
      this.challengeService.getUserChallengePeriodPerformance(),
      this.tenantSettingsService.get(),
    ]).subscribe({
      next: ([periodPerformance, tenantSettings]) => {
        this.periodPerformance = periodPerformance;
        this.tenantSettings = tenantSettings;

        if (this.periodPerformance && !this.tenantSettings.isCustomPeriodOn) {
          combineLatest([
            this.challengeService.getUserChallengeList(),
            this.rewardsService.getUserChallengePeriodRewardList(
              this.periodPerformance.id
            ),
            this.challengeService.getUserUniversalChallengeList(),
          ]).subscribe({
            next: ([
              userChallengeList,
              rewardList,
              userUniversalChallenges,
            ]) => {
              this.userChallengeList = userChallengeList;
              this.userChallengePeriodRewardList = rewardList;
              this.userUniversalChallengeList = userUniversalChallenges ?? [];
              this.initUserUniversalChallengesAnimation();

              // Required to detect the changes from the api. Otherwise dom is empty
              // Force the DOM Update Before Querying
              this.cd.detectChanges();
              this.runChallengeAnimation();
              this.updateRewardProgressBar();

              this.initProgressContainerWidth();

              this.resetRewardsInactivityTimer();
              this.resetPointsInactivityTimer();

              if (this.rewardsScroller) {
                this.rewardsListener = this.renderer.listen(
                  this.rewardsScroller.nativeElement,
                  'scroll',
                  () => {
                    if (!this.rewardsScrolling) {
                      this.rewardsScrolling = true;
                      this.resetRewardsInactivityTimer();
                      // Trigger any function here
                    }
                  }
                );
              }

              if (this.pointsScroller) {
                this.pointsListener = this.renderer.listen(
                  this.pointsScroller.nativeElement,
                  'scroll',
                  () => {
                    if (!this.pointsScrolling) {
                      this.pointsScrolling = true;
                      this.resetPointsInactivityTimer();
                      // Trigger any function here
                    }
                  }
                );
              }

              this.loadingContext.disableManualMode();
              this.loadingService.loadingOff();
            },
          });
        } else if (
          this.periodPerformance &&
          this.tenantSettings.isCustomPeriodOn
        ) {
          this.challengeService.getUserCustomPeriod().subscribe({
            next: (customPeriod) => {
              if (customPeriod) {
                this.userCustomPeriodChallengesList = customPeriod.challenges;
                this.userCustomPeriodRewardsList = customPeriod.rewards;
                this.userUniversalChallengeList =
                  customPeriod.universalChallenges;
                this.initUserUniversalChallengesAnimation();

                // Required to detect the changes from the api. Otherwise dom is empty
                // Force the DOM Update Before Querying
                this.cd.detectChanges();
                this.runChallengeAnimation();
                this.updateRewardProgressBar();

                this.initProgressContainerWidth();

                this.resetRewardsInactivityTimer();
                this.resetPointsInactivityTimer();

                if (this.rewardsScroller) {
                  this.rewardsListener = this.renderer.listen(
                    this.rewardsScroller.nativeElement,
                    'scroll',
                    () => {
                      if (!this.rewardsScrolling) {
                        this.rewardsScrolling = true;
                        this.resetRewardsInactivityTimer();
                        // Trigger any function here
                      }
                    }
                  );
                }

                if (this.pointsScroller) {
                  this.pointsListener = this.renderer.listen(
                    this.pointsScroller.nativeElement,
                    'scroll',
                    () => {
                      if (!this.pointsScrolling) {
                        this.pointsScrolling = true;
                        this.resetPointsInactivityTimer();
                        // Trigger any function here
                      }
                    }
                  );
                }

                this.loadingContext.disableManualMode();
                this.loadingService.loadingOff();
              }
            },
          });
        }
      },
    });
  }

  public openUniversalChallengeBuyItemsQrCode(): void {
    this.dialog.open(QrCodeDialog, {
      width: '17rem',
      data: {
        Id: 14,
        Name: this.translateService.instant(
          'qr_scanner.enum_values.PurchaseChallenge'
        ),
        Type: QrCodeType.PurchaseChallenge,
        AdditionalData: {
          userId: this.authService.getUser?.id,
        },
      },
    });
  }

  public getLeftOffset(requiredPoints: number, index: number): number {
    const rewards = this.tenantSettings?.isCustomPeriodOn
      ? this.userCustomPeriodRewardsList
      : this.userChallengePeriodRewardList;
    const maxPoints = rewards[rewards.length - 1].rewardRequiredPoints;

    let offset = (requiredPoints / maxPoints) * 100;

    // Check if previous reward is close
    if (index > 0) {
      const prevPoints = rewards[index - 1].rewardRequiredPoints;
      const prevOffset = (prevPoints / maxPoints) * 100;

      const minDistance = 10; // Minimum % spacing between circles

      if (offset - prevOffset < minDistance) {
        offset = prevOffset + minDistance;
      }
    }

    return offset - 5;
  }

  private resetRewardsInactivityTimer(): void {
    this.inactivityRewardsTimer?.unsubscribe(); // Clear existing timer if any
    this.rewardScrollArrowsOpacity = 1; // Make the button fully visible
    this.rewardScrollArrowsWidth = this.scrollArrowWidth;
    this.inactivityRewardsTimer = timer(3000).subscribe(() => {
      this.rewardScrollArrowsOpacity = 0.5; // Reduce opacity after 3 seconds of inactivity
      this.rewardScrollArrowsWidth = 1;
      this.rewardsScrolling = false;
    });
  }

  private resetPointsInactivityTimer(): void {
    this.inactivityPointsTimer?.unsubscribe(); // Clear existing timer if any
    this.pointsScrollArrowsOpacity = 1; // Make the button fully visible
    this.pointsScrollArrowsWidth = this.scrollArrowWidth;
    this.inactivityPointsTimer = timer(3000).subscribe(() => {
      this.pointsScrollArrowsOpacity = 0.5; // Reduce opacity after 3 seconds of inactivity
      this.pointsScrollArrowsWidth = 1;
      this.pointsScrolling = false;
    });
  }

  public rewardsScrollLeft(): void {
    this.resetRewardsInactivityTimer();
    this.rewardsScroller.nativeElement.scrollBy({
      left: -150,
      behavior: 'smooth',
    });
  }

  public rewardsScrollRight(): void {
    this.resetRewardsInactivityTimer();
    this.rewardsScroller.nativeElement.scrollBy({
      left: 150,
      behavior: 'smooth',
    });
  }

  public pointsScrollLeft(): void {
    this.resetPointsInactivityTimer();
    this.pointsScroller.nativeElement.scrollBy({
      left: -150,
      behavior: 'smooth',
    });
  }

  public pointsScrollRight(): void {
    this.resetPointsInactivityTimer();
    this.pointsScroller.nativeElement.scrollBy({
      left: 150,
      behavior: 'smooth',
    });
  }

  private updateRewardProgressBar() {
    const progress: HTMLElement | null = document.getElementById('progress');
    const stepCircles: NodeListOf<Element> =
      document.querySelectorAll('.circle');

    const totalPoints = this.periodPerformance?.points ?? 0;
    let rewards = this.tenantSettings?.isCustomPeriodOn
      ? this.userCustomPeriodRewardsList
      : this.userChallengePeriodRewardList;

    if (!rewards.length) return;

    // ✅ Set progress width based on points (not reward index)
    const maxPoints = rewards[rewards.length - 1].rewardRequiredPoints;
    const clampedPoints = Math.min(totalPoints, maxPoints);
    let width = (clampedPoints / maxPoints) * 100;
    if (totalPoints !== 0) {
      width += 5;
    }
    if (progress) {
      progress.style.width = `${width}%`;
    }

    // 🟢 Update reward circles (optional)
    rewards.forEach((reward, i) => {
      const circle = stepCircles[i];
      if (!circle) return;

      if (totalPoints >= reward.rewardRequiredPoints) {
        circle.classList.add('activated');
        circle.innerHTML = `<img class="_img" src="/shared/assets/images/challenge-complete.png" alt="" />`;
      } else {
        circle.classList.remove('activated');
        circle.innerHTML = reward.rewardRequiredPoints.toString();
      }
    });
  }

  private initProgressContainerWidth(): void {
    // Get the count of circle elements
    const numberOfCircles = this.circles.length;

    // Calculate the width of each circle
    if (numberOfCircles > 0) {
      const circleWidth = 4 * numberOfCircles;

      this.progressContainer.nativeElement.style.width = `${circleWidth}rem`;
    }
  }

  private runChallengeAnimation(): void {
    // Reset first
    this.animateChallengeProgress = false;

    // Trigger animation after DOM rendered
    setTimeout(() => {
      this.animateChallengeProgress = true;
    }, 0);
  }

  private initUserUniversalChallengesAnimation(): void {
    this.userUniversalChallengeList.forEach((_, i) => {
      const interval = setInterval(() => {
        // Random chance to shake
        if (Math.random() < 0.3) {
          // 30% chance every interval
          this.shakeStates[i] = true;

          // Remove shake after animation duration
          setTimeout(() => (this.shakeStates[i] = false), 1500);
        }
      }, 3000 + Math.random() * 10000); // random 2–5 seconds
      this.intervals.push(interval);
    });
  }
}
