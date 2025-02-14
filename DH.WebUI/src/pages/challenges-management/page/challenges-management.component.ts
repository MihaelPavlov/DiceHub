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

@Component({
  selector: 'app-challenges-management',
  templateUrl: 'challenges-management.component.html',
  styleUrl: 'challenges-management.component.scss',
})
export class ChallengesManagementComponent implements OnInit, OnDestroy {
  @ViewChild('rewardsScroller') rewardsScroller!: ElementRef;
  @ViewChild('pointsScroller') pointsScroller!: ElementRef;

  @ViewChild('progressContainer') progressContainer!: ElementRef; // Reference to the container
  @ViewChildren('circleContainer') circles!: QueryList<any>; // Query list of circles

  private readonly scrollArrowWidth = 2.5;
  public rewardScrollArrowsOpacity: number = 1;
  public rewardScrollArrowsWidth: number = this.scrollArrowWidth;
  public pointsScrollArrowsOpacity: number = 1;
  public pointsScrollArrowsWidth: number = this.scrollArrowWidth;
  public periodPerformance!: IUserChallengePeriodPerformance;
  public userChallengePeriodRewardList!: IUserChallengePeriodReward[];
  public userChallengeList: IUserChallenge[] = [];
  public ChallengeStatus = ChallengeStatus;
  public readonly ImageEntityType = ImageEntityType;
  public readonly DATE_TIME_FORMAT = DateHelper.DATE_TIME_FORMAT;

  public columns: Column[] = [
    // TODO: Update the word weekly based on the settings. Weekly rewards could be monthly
    {
      name: 'Weekly',
      link: FULL_ROUTE.CHALLENGES.CHALLENGES_HOME,
      isActive: true,
    },
    {
      name: 'Rewards',
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
    private readonly cd: ChangeDetectorRef,
    private renderer: Renderer2
  ) {}

  public ngOnDestroy(): void {
    if (this.rewardsListener) this.rewardsListener();
    if (this.pointsListener) this.pointsListener();
  }

  private initProgressContainerWidth(): void {
    console.log('progressContainer', this.progressContainer);

    // Get the count of circle elements
    const numberOfCircles = this.circles.length;
    console.log('numberOfCircles', numberOfCircles);

    // Calculate the width of each circle
    if (numberOfCircles > 0) {
      const circleWidth = 4 * numberOfCircles;
      console.log('circle width -> ', circleWidth);

      this.progressContainer.nativeElement.style.width = `${circleWidth}rem`;
    }
  }

  public ngOnInit(): void {
    this.challengeService.getUserChallengePeriodPerformance().subscribe({
      next: (periodPerformance: IUserChallengePeriodPerformance) => {
        this.periodPerformance = periodPerformance;

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
          },
        });
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

  private updateRewardProgressBar() {
    const progress: HTMLElement | null = document.getElementById('progress');
    const stepCircles: NodeListOf<Element> =
      document.querySelectorAll('.circle');
    let currentActive: number = this.userChallengePeriodRewardList.filter(
      (reward) => reward.isCompleted
    ).length;

    stepCircles.forEach((circle: Element, i: number) => {
      if (i < currentActive) {
        circle.classList.add('activated');
        circle.innerHTML = `
          <img class="_img"
          src="/shared/assets/images/challenge-complete.png"
          alt=""
          />`;
        circle.classList.add('_img');
      } else {
        circle.classList.remove('activated');
      }
    });

    const activeCircles: NodeListOf<Element> =
      document.querySelectorAll('.activated');
    console.log('activated circles --- >', activeCircles);
    if (progress && stepCircles.length > 1) {
      console.log(
        ((activeCircles.length - 1) / (stepCircles.length - 1)) * 100 + '%'
      );

      progress.style.width =
        ((activeCircles.length - 1) / (stepCircles.length - 1)) * 100 + '%';
    }
  }
}
