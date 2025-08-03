import { BehaviorSubject, Observable } from 'rxjs';
import {
  GameActivityStats,
  GetGameActivityChartData,
} from './../../../../entities/statistics/models/game-activity-chart.model';
import {
  Component,
  AfterViewInit,
  OnDestroy,
  ChangeDetectorRef,
} from '@angular/core';
import { Router } from '@angular/router';
import { StatisticsService } from '../../../../entities/statistics/api/statistics.service';
import { NAV_ITEM_LABELS } from '../../../../shared/models/nav-items-labels.const';
import { MenuTabsService } from '../../../../shared/services/menu-tabs.service';
import { ToastService } from '../../../../shared/services/toast.service';
import { FormControl } from '@angular/forms';
import { IDropdown } from '../../../../shared/models/dropdown.model';
import { addDays, addMonths, addYears, format } from 'date-fns';
import { GamesActivityType } from '../../../../entities/statistics/enums/games-activity-type.enum';
import { OperationResult } from '../../../../shared/models/operation-result.model';
import { ImageEntityType } from '../../../../shared/pipe/entity-image.pipe';
import {
  GameUserActivity,
  GetUsersWhoPlayedGameData,
} from '../../../../entities/statistics/models/game-user-activity.model';
import { AppToastMessage } from '../../../../shared/components/toast/constants/app-toast-messages.constant';
import { ToastType } from '../../../../shared/models/toast.model';
import {
  trigger,
  state,
  style,
  transition,
  animate,
} from '@angular/animations';
import { DateHelper } from '../../../../shared/helpers/date-helper';

@Component({
  selector: 'games-chart',
  templateUrl: 'games-chart.component.html',
  styleUrl: 'games-chart.component.scss',
  animations: [
    trigger('slideToggle', [
      transition(':enter', [
        style({ height: 0, opacity: 0 }),
        animate('300ms ease-out', style({ height: '*', opacity: 1 })),
      ]),
      transition(':leave', [
        animate('300ms ease-in', style({ height: 0, opacity: 0 })),
      ]),
    ]),
  ],
})
export class GamesChartComponent implements AfterViewInit, OnDestroy {
  public chartType: IDropdown[] = [];
  public gamesChartType = new FormControl<number | null>(0);
  public currentRangeStart?: Date | null = null;
  public currentRangeEnd?: Date | null = null;
  public readonly GamesActivityType = GamesActivityType;
  public readonly ImageEntityType = ImageEntityType;
  public selectedGameId: number | null = null;
  public readonly DATE_TIME_FORMAT: string = DateHelper.DATE_TIME_FORMAT;

  public gamesActivityDataSubject: BehaviorSubject<GameActivityStats[]> =
    new BehaviorSubject<GameActivityStats[]>([]);

  public selectedGameUsers: BehaviorSubject<GameUserActivity[]> =
    new BehaviorSubject<GameUserActivity[]>([]);

  constructor(
    private readonly menuTabsService: MenuTabsService,
    private readonly toastService: ToastService,
    private readonly router: Router,
    private readonly statisticsService: StatisticsService,
    private readonly cd: ChangeDetectorRef
  ) {
    this.chartType = Object.entries(GamesActivityType)
      .filter(([key, value]) => typeof value === 'number')
      .map(([key, value]) => ({ id: value as number, name: key }));

    this.menuTabsService.setActive(NAV_ITEM_LABELS.PROFILE);
    this.gamesChartType.valueChanges.subscribe(() => this.resetDateRange());
  }
  public ngAfterViewInit(): void {
    this.currentRangeStart = this.getStartOfWeek();
    this.currentRangeEnd = addDays(this.currentRangeStart, 6);
    this.getGameActivityData(GamesActivityType.Weekly);
  }

  public ngOnDestroy(): void {
    this.menuTabsService.resetData();
  }

  public backNavigateBtn(): void {
    this.router.navigateByUrl('profile');
  }

  public onClickGame(gameId: number): void {
    if (this.selectedGameId === gameId) {
      // Toggle off
      this.selectedGameId = null;
      this.selectedGameUsers.next([]);
      return;
    }
    this.selectedGameId = gameId;

    this.statisticsService
      .getGameUserActivityChartData(
        this.selectedGameId,
        this.gamesChartType.value as GamesActivityType,
        this.currentRangeStart,
        this.currentRangeEnd
      )
      .subscribe({
        next: (
          operation: OperationResult<GetUsersWhoPlayedGameData> | null
        ) => {
          if (operation && operation.success && operation.relatedObject) {
            console.log(
              'Game User Activity Data:',
              operation.relatedObject.users
            );

            this.selectedGameUsers.next(operation.relatedObject.users);
          }
        },
        error: (error) => {
          this.toastService.error({
            message: AppToastMessage.SomethingWrong,
            type: ToastType.Error,
          });
        },
      });
  }

  private getGameActivityData(type: GamesActivityType): void {
    this.statisticsService
      .getGameActivityChartData(
        type,
        this.currentRangeStart,
        this.currentRangeEnd
      )
      .subscribe({
        next: (operation: OperationResult<GetGameActivityChartData> | null) => {
          if (operation && operation.success && operation.relatedObject) {
            this.gamesActivityDataSubject.next(operation.relatedObject.games);
          }
        },
        error: (error) => {
          this.toastService.error({
            message: AppToastMessage.SomethingWrong,
            type: ToastType.Error,
          });
        },
      });
  }

  private resetDateRange(): void {
    const selectedType = this.gamesChartType.value as GamesActivityType | null;
    console.log(`Selected Type: ${this.gamesChartType.value}`);

    // Toggle off
    this.selectedGameId = null;

    if (selectedType === GamesActivityType.Weekly) {
      this.currentRangeStart = this.getStartOfWeek();
      this.currentRangeEnd = addDays(this.currentRangeStart, 6);
    } else if (selectedType === GamesActivityType.Monthly) {
      this.currentRangeStart = new Date();
      this.currentRangeEnd = addMonths(this.currentRangeStart, 1);
    } else if (selectedType === GamesActivityType.Yearly) {
      this.currentRangeStart = new Date();
      this.currentRangeEnd = addYears(this.currentRangeStart, 1);
    } else if (selectedType === GamesActivityType.AllTime) {
      this.currentRangeStart = null;
      this.currentRangeEnd = null;
    } else {
      this.gamesActivityDataSubject.next([]);
      return;
    }

    this.getGameActivityData(selectedType);
  }

  public getFormattedRange(): string {
    let date;
    const selectedType = this.gamesChartType.value;

    if (
      selectedType === (GamesActivityType.Weekly as number) &&
      this.currentRangeStart &&
      this.currentRangeEnd
    ) {
      date = `${format(this.currentRangeStart, 'MMM dd yyyy')} - ${format(
        this.currentRangeEnd,
        'MMM dd yyyy'
      )}`;
    } else if (
      selectedType === GamesActivityType.Monthly &&
      this.currentRangeStart
    ) {
      date = `${format(this.currentRangeStart, 'MMM yyyy')}`;
    } else if (
      selectedType === GamesActivityType.Yearly &&
      this.currentRangeStart
    ) {
      date = `${format(this.currentRangeStart, 'yyyy')}`;
    }

    return date;
  }
  public updateDateRange(direction: 'forward' | 'backward'): void {
    const selectedType = this.gamesChartType.value as GamesActivityType;
    // Toggle off
    this.selectedGameId = null;
    let adjustmentValue = 0;
    if (selectedType === GamesActivityType.Weekly)
      adjustmentValue = 7; // Weekly
    else if (selectedType === GamesActivityType.Monthly)
      adjustmentValue = 1; // Monthly
    else if (selectedType === GamesActivityType.Yearly)
      adjustmentValue = 1; // Yearly
    else if (selectedType === GamesActivityType.AllTime) adjustmentValue = 0; // All Time

    if (direction === 'backward') adjustmentValue *= -1;

    if (
      selectedType === GamesActivityType.Weekly &&
      this.currentRangeStart &&
      this.currentRangeEnd
    ) {
      this.currentRangeStart = addDays(this.currentRangeStart, adjustmentValue);
      this.currentRangeEnd = addDays(this.currentRangeEnd, adjustmentValue);
    } else if (
      selectedType === GamesActivityType.Monthly &&
      this.currentRangeStart
    ) {
      this.currentRangeStart = addMonths(
        this.currentRangeStart,
        adjustmentValue
      );
      this.currentRangeEnd = null;
    } else if (
      selectedType === GamesActivityType.Yearly &&
      this.currentRangeStart
    ) {
      this.currentRangeStart = addYears(
        this.currentRangeStart,
        adjustmentValue
      );
      this.currentRangeEnd = null;
    } else {
      return;
    }

    this.getGameActivityData(selectedType);
  }

  private getStartOfWeek(): Date {
    const date = new Date();
    const day = date.getDay();
    const diff = day === 0 ? -6 : 1 - day; // Adjust for Sunday (0) and other days
    const startOfWeek = new Date(date);
    startOfWeek.setDate(date.getDate() + diff);
    startOfWeek.setHours(0, 0, 0, 0); // Set time to 00:00:00.000
    return startOfWeek;
  }
}
