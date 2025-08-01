import {
  Component,
  AfterViewInit,
  OnDestroy,
  ViewChild,
  ElementRef,
} from '@angular/core';
import { Router } from '@angular/router';
import { Chart, registerables } from 'chart.js';
import { StatisticsService } from '../../../../entities/statistics/api/statistics.service';
import { NAV_ITEM_LABELS } from '../../../../shared/models/nav-items-labels.const';
import { MenuTabsService } from '../../../../shared/services/menu-tabs.service';
import { ToastService } from '../../../../shared/services/toast.service';
import ChartDataLabels from 'chartjs-plugin-datalabels';
import { FormControl } from '@angular/forms';
import { IDropdown } from '../../../../shared/models/dropdown.model';
import { addDays, addMonths, addYears, format } from 'date-fns';
import { GamesActivityType } from '../../../../entities/statistics/enums/games-activity-type.enum';
type Game = { name: string; value: number; image?: string | null };

@Component({
  selector: 'games-chart',
  templateUrl: 'games-chart.component.html',
  styleUrl: 'games-chart.component.scss',
})
export class GamesChartComponent implements AfterViewInit, OnDestroy {
  public chartType: IDropdown[] = [];
  public gamesChartType = new FormControl(0);
  public currentRangeStart: Date = this.getStartOfWeek();
  public currentRangeEnd: Date = addDays(this.currentRangeStart, 6);
  public GamesActivityType = GamesActivityType;

  public games: Game[] = [
    {
      name: 'Exploding Kittens',
      value: 20,
      image: 'shared/assets/images/landing_image_1.png',
    },
    { name: 'Monopoly', value: 30 },
    { name: 'Chess', value: 25 },
    { name: 'Scrabble', value: 20 },
    { name: 'Uno', value: 15 },
    { name: 'Settlers of Catan', value: 10 },
    { name: 'Ticket to Ride', value: 5 },
    {
      name: 'Pandemic',
      value: 43,
      image: 'shared/assets/images/landing_image_1.png',
    },
    { name: 'Carcassonne', value: 22 },
    { name: 'Codenames', value: 51 },
    { name: 'Risk', value: 14 },
    { name: 'Clue', value: 32 },
    { name: 'Battleship', value: 1 },
    { name: 'Jenga', value: 23 },
    { name: 'Pictionary', value: 1 },
    { name: 'Twilight Struggle', value: 1 },
    { name: '7 Wonders', value: 1 },
    { name: 'Dominion', value: 1 },
    { name: 'Azul', value: 1 },
    { name: 'Terraforming Mars', value: 1 },
    { name: 'Gloomhaven', value: 1 },
    { name: 'Wingspan', value: 1 },
    { name: 'Root', value: 1 },
    { name: 'Star Realms', value: 1 },
    { name: 'Magic: The Gathering', value: 1 },
    { name: 'Dungeons & Dragons', value: 1 },
    { name: 'Pokemon TCG', value: 1 },
    { name: 'Yu-Gi-Oh!', value: 1 },
    { name: 'Hearthstone', value: 1 },
    { name: 'League of Legends', value: 1 },
    { name: 'Counter-Strike', value: 1 },
    { name: 'Fortnite', value: 1 },
    { name: 'Call of Duty', value: 1 },
    { name: 'Apex Legends', value: 1 },
    { name: 'Valorant', value: 1 },
    { name: 'Overwatch', value: 1 },
    { name: 'World of Warcraft', value: 1 },
    { name: 'Final Fantasy XIV', value: 1 },
    { name: 'Minecraft', value: 1 },
    { name: 'Roblox', value: 1 },
    // ...more games
  ];
  public myImage = new Image();

  constructor(
    private readonly menuTabsService: MenuTabsService,
    private readonly toastService: ToastService,
    private readonly router: Router,
    private readonly statisticsService: StatisticsService
  ) {

    this.chartType = Object.entries(GamesActivityType)
      .filter(([key, value]) => typeof value === 'number')
      .map(([key, value]) => ({ id: value as number, name: key }));

    this.menuTabsService.setActive(NAV_ITEM_LABELS.PROFILE);
    this.gamesChartType.valueChanges.subscribe(() => this.resetDateRange());
  }
  public async ngAfterViewInit(): Promise<void> {

  }

  public ngOnDestroy(): void {
    this.menuTabsService.resetData();
  }

  public backNavigateBtn(): void {
    this.router.navigateByUrl('profile');
  }



  private resetDateRange(): void {
    const selectedType = this.gamesChartType.value;

    // reset games
    // if (this.gamesActivityChart) {
    //   this.gamesActivityChart.destroy();
    // }
    if (selectedType === GamesActivityType.Weekly) {
      this.currentRangeStart = this.getStartOfWeek();
      this.currentRangeEnd = addDays(this.currentRangeStart, 6);

      // this.createVisitorWeekActivityChartCanvas(colors);
    } else if (selectedType === GamesActivityType.Monthly) {
      this.currentRangeStart = new Date();
      this.currentRangeEnd = addMonths(this.currentRangeStart, 1);

      // this.createVisitorMonthActivityChartCanvas(colors);
    } else if (selectedType === GamesActivityType.Yearly) {
      this.currentRangeStart = new Date();
      this.currentRangeEnd = addYears(this.currentRangeStart, 1);

      // this.createVisitorYearlyActivityChartCanvas(colors);
    }
  }
  public getFormattedRange(): string {
    let date;
    const selectedType = this.gamesChartType.value;

    if (selectedType === (GamesActivityType.Weekly as number)) {
      date = `${format(this.currentRangeStart, 'MMM dd yyyy')} - ${format(
        this.currentRangeEnd,
        'MMM dd yyyy'
      )}`;
    } else if (selectedType === GamesActivityType.Monthly) {
      date = `${format(this.currentRangeStart, 'MMM yyyy')}`;
    } else if (selectedType === GamesActivityType.Yearly) {
      date = `${format(this.currentRangeStart, 'yyyy')}`;
    }

    return date;
  }
  public updateDateRange(direction: 'forward' | 'backward'): void {
    const selectedType = this.gamesChartType.value;
   //TODO: Reset games
    // if (this.gamesActivityChart) {
    //   this.gamesActivityChart.destroy();
    // }
    let adjustmentValue = 0;
    if (selectedType === GamesActivityType.Weekly)
      adjustmentValue = 7; // Weekly
    else if (selectedType === GamesActivityType.Monthly)
      adjustmentValue = 1; // Monthly
    else if (selectedType === GamesActivityType.Yearly)
      adjustmentValue = 1; // Yearly
    else if (selectedType === GamesActivityType.AllTime) adjustmentValue = 0; // All Time

    if (direction === 'backward') adjustmentValue *= -1;

    if (selectedType === GamesActivityType.Weekly) {
      this.currentRangeStart = addDays(this.currentRangeStart, adjustmentValue);
      this.currentRangeEnd = addDays(this.currentRangeEnd, adjustmentValue);

      //   this.createVisitorWeekActivityChartCanvas(colors);
    } else if (selectedType === GamesActivityType.Monthly) {
      this.currentRangeStart = addMonths(
        this.currentRangeStart,
        adjustmentValue
      );

      //   this.createVisitorMonthActivityChartCanvas(colors);
    } else if (selectedType === GamesActivityType.Yearly) {
      this.currentRangeStart = addYears(
        this.currentRangeStart,
        adjustmentValue
      );

      //   this.createVisitorYearlyActivityChartCanvas(colors);
    } else if (selectedType === GamesActivityType.AllTime) {
      //   this.createVisitorYearlyActivityChartCanvas(colors);
    }
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
