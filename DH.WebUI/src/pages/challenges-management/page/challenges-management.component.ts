import { IUserChallengePeriodReward } from './../../../entities/rewards/models/user-period-reward.model';
import { IUserChallenge } from './../../../entities/challenges/models/user-challenge.model';
import {
  Component,
  OnInit,
  ChangeDetectorRef,
  ViewChild,
  ElementRef,
} from '@angular/core';
import { ChallengesService } from '../../../entities/challenges/api/challenges.service';
import { RewardsService } from '../../../entities/rewards/api/rewards.service';
import { IUserChallengePeriodPerformance } from '../../../entities/challenges/models/user-challenge-period-performance.model';
import { combineLatest } from 'rxjs';
import { ChallengeStatus } from '../../../entities/challenges/enums/challenge-status.enum';
import { Column } from '../../../widgets/nav-bar/page/nav-bar.component';
import { FULL_ROUTE } from '../../../shared/configs/route.config';
import { ImageEntityType } from '../../../shared/pipe/entity-image.pipe';

@Component({
  selector: 'app-challenges-management',
  templateUrl: 'challenges-management.component.html',
  styleUrl: 'challenges-management.component.scss',
})
export class ChallengesManagementComponent implements OnInit {
  @ViewChild('scroller') scroller!: ElementRef;

  public periodPerformance!: IUserChallengePeriodPerformance;
  public userChallengePeriodRewardList!: IUserChallengePeriodReward[];
  public userChallengeList: IUserChallenge[] = [];
  public ChallengeStatus = ChallengeStatus;
  public readonly ImageEntityType = ImageEntityType;

  public columns: Column[] = [
    {
      name: 'Daily Task',
      link: FULL_ROUTE.CHALLENGES.CHALLENGES_HOME,
      isActive: true,
    },
    // TODO: Update the word weekly based on the settings. Weekly rewards could be monthly
    { name: 'Weekly', link: '', isActive: false },
    {
      name: 'Rewards',
      link: FULL_ROUTE.CHALLENGES.CHALLENGES_REWARDS,
      isActive: false,
    },
  ];

  constructor(
    private readonly rewardsService: RewardsService,
    private readonly challengeService: ChallengesService,
    private readonly cd: ChangeDetectorRef
  ) {}

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
          },
        });
      },
    });
  }

  public scrollLeft(): void {
    this.scroller.nativeElement.scrollBy({ left: -150, behavior: 'smooth' });
  }

  public scrollRight(): void {
    this.scroller.nativeElement.scrollBy({ left: 150, behavior: 'smooth' });
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
          src="../../../shared/assets/images/challenge-complete.png"
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
