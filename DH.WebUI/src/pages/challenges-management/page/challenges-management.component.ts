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

@Component({
  selector: 'app-challenges-management',
  templateUrl: 'challenges-management.component.html',
  styleUrl: 'challenges-management.component.scss',
})
export class ChallengesManagementComponent implements OnInit, OnDestroy {
  @ViewChild('rewardsScroller') rewardsScroller!: ElementRef;
  @ViewChild('pointsScroller') pointsScroller!: ElementRef;

  @ViewChild('progressContainer') progressContainer!: ElementRef;
  @ViewChildren('circleContainer') circles!: QueryList<any>;
  activeTab: 'game' | 'universal' = 'game';
 switchTab(tab: 'game' | 'universal') {
    this.activeTab = tab;
  }
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

  public ChallengeStatus = ChallengeStatus;
  public readonly ImageEntityType = ImageEntityType;
  public readonly DATE_FORMAT = DateHelper.DATE_FORMAT;

  public columns: Column[] = [
    // TODO: Update the word weekly based on the settings. Weekly rewards could be monthly
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
  ];

  private inactivityRewardsTimer!: Subscription;
  private inactivityPointsTimer!: Subscription;
  private rewardsScrolling = false;
  private pointsScrolling = false;
  private rewardsListener!: () => void;
  private pointsListener!: () => void;

  constructor(
    private readonly rewardsService: RewardsService,
    private readonly challengeService: ChallengesService,
    private readonly tenantSettingsService: TenantSettingsService,
    private readonly cd: ChangeDetectorRef,
    private renderer: Renderer2,
    private readonly dialog: MatDialog,
    private readonly loadingContext: LoadingInterceptorContextService,
    private readonly loadingService: LoadingService,
    private readonly translateService: TranslateService
  ) {}

  public ngOnDestroy(): void {
    if (this.rewardsListener) this.rewardsListener();
    if (this.pointsListener) this.pointsListener();
  }

  public openImagePreview(imageUrl: string) {
    this.dialog.open<ImagePreviewDialog, ImagePreviewData>(ImagePreviewDialog, {
      data: {
        imageUrl,
        title: 'Image',
      },
      width: '17rem',
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

  public ngOnInit(): void {
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
          ]).subscribe({
            next: ([userChallengeList, rewardList]) => {
              this.userChallengeList = userChallengeList;
              this.userChallengePeriodRewardList = rewardList;

              // Required to detect the changes from the api. Otherwise dom is empty
              // Force the DOM Update Before Querying
              this.cd.detectChanges();
              this.updateChallengesProgressBar();
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

                // Required to detect the changes from the api. Otherwise dom is empty
                // Force the DOM Update Before Querying
                this.cd.detectChanges();
                this.updateChallengesProgressBar();
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

  private updateChallengesProgressBar(): void {
    if (this.tenantSettings?.isCustomPeriodOn) {
      this.userCustomPeriodChallengesList.forEach((challenge, index) => {
        const progressPercentage =
          (challenge.currentAttempts / challenge.challengeAttempts) * 100;

        const barValue = document.getElementById(
          `progress-bar-${index}`
        ) as HTMLElement;

        const style = document.createElement('style');
        style.textContent = `
          @keyframes custom-load-${index} {
            0% {
              width: 0%;
            }
            100% {
              width: ${progressPercentage}%;
            }
          }
        `;

        document.head.appendChild(style);

        barValue.style.animation = `custom-load-${index} 3s normal forwards`;
      });
    } else {
      this.userChallengeList.forEach((challenge, index) => {
        const progressPercentage =
          (challenge.currentAttempts / challenge.maxAttempts) * 100;

        const barValue = document.getElementById(
          `progress-bar-${index}`
        ) as HTMLElement;

        const style = document.createElement('style');
        style.textContent = `
          @keyframes custom-load-${index} {
            0% {
              width: 0%;
            }
            100% {
              width: ${progressPercentage}%;
            }
          }
        `;

        document.head.appendChild(style);

        barValue.style.animation = `custom-load-${index} 3s normal forwards`;
      });
    }
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
}
