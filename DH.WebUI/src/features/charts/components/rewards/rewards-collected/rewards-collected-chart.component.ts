import {
  Component,
  AfterViewInit,
  OnDestroy,
  ViewChild,
  ElementRef,
  ChangeDetectorRef,
} from '@angular/core';
import ChartDataLabels from 'chartjs-plugin-datalabels';
import { Router } from '@angular/router';
import { Chart, registerables } from 'chart.js';
import { NAV_ITEM_LABELS } from '../../../../../shared/models/nav-items-labels.const';
import { LoadingService } from '../../../../../shared/services/loading.service';
import { MenuTabsService } from '../../../../../shared/services/menu-tabs.service';
import { colors } from '../../../consts/colors.const';
import { FormControl } from '@angular/forms';
import { ControlsMenuComponent } from '../../../../../shared/components/menu/controls-menu.component';
interface IRewardsDataChart {
  rewardName: string;
  collectionCount: number;
}
@Component({
  selector: 'rewards-collected-chart',
  templateUrl: 'rewards-collected-chart.component.html',
  styleUrl: 'rewards-collected-chart.component.scss',
})
export class RewardsCollectedChartComponent
  implements AfterViewInit, OnDestroy
{
  @ViewChild('rewardsChartCanvas')
  private rewardsChartCanvas!: ElementRef<HTMLCanvasElement>;
  private rewardsChart: any;
  public fromDateControl = new FormControl(null);
  public toDateControl = new FormControl(null);
  public rewardsCount: number = 10;

  constructor(
    private readonly loadingService: LoadingService,
    private readonly router: Router,
    private readonly menuTabsService: MenuTabsService,
    private readonly cd : ChangeDetectorRef
  ) {
    this.menuTabsService.setActive(NAV_ITEM_LABELS.PROFILE);

    Chart.register(ChartDataLabels, ...registerables);
    this.toDateControl.valueChanges.subscribe(() => this.dateValidation());
  }

  public dateValidation(): void {
    // if toDateControl is changes, check if fromDateControl have value
    // if not
    //    show message the specifying from which date we start is important
    // If yes
    //    validate if the fromDateControl.value is smaller then toDateControl.value
  }

  public ngOnDestroy(): void {
    this.menuTabsService.resetData();
  }

  public ngAfterViewInit(): void {
    this.createRewardsStatsChartCanvas(colors);
  }

  public backNavigateBtn(): void {
    this.router.navigateByUrl('charts/rewards');
  }

  public showMenu(
    event: MouseEvent,
    controlMenu: ControlsMenuComponent
  ): void {
    event.stopPropagation();
    controlMenu.toggleMenu();
  }

  public createRewardsStatsChartCanvas(colors): void {
    const ctx = this.rewardsChartCanvas.nativeElement.getContext('2d');
    if (!ctx) return;

    const rewardsData: IRewardsDataChart[] = [
      { rewardName: 'Reward long long long long', collectionCount: 191 },
      { rewardName: 'Reward F', collectionCount: 186 },
      { rewardName: 'Reward T', collectionCount: 180 },
      { rewardName: 'Reward E', collectionCount: 180 },
      { rewardName: 'Reward B', collectionCount: 161 },
      { rewardName: 'Reward L', collectionCount: 160 },
      { rewardName: 'Reward S', collectionCount: 157 },
      { rewardName: 'Reward J', collectionCount: 148 },
      { rewardName: 'Reward N', collectionCount: 134 },
      { rewardName: 'Reward P', collectionCount: 119 },
      { rewardName: 'Reward M', collectionCount: 110 },
      { rewardName: 'Reward O', collectionCount: 110 },
      { rewardName: 'Reward Q', collectionCount: 103 },
      { rewardName: 'Reward H', collectionCount: 100 },
      { rewardName: 'Reward A', collectionCount: 100 },
      { rewardName: 'Reward G', collectionCount: 94 },
      { rewardName: 'Reward C', collectionCount: 84 },
      { rewardName: 'Reward I', collectionCount: 78 },
      { rewardName: 'Reward D', collectionCount: 76 },
    ];

    this.rewardsCount = rewardsData.length;
    this.cd.detectChanges();
    let gradientPurple = ctx.createLinearGradient(0, 25, 0, 300);
    gradientPurple.addColorStop(0.35, colors.fadedPeach.half);
    gradientPurple.addColorStop(1, colors.fadedPeach.quarter);
    this.rewardsChart = new Chart(ctx, {
      type: 'bar',
      data: {
        labels: rewardsData.map((data) => data.rewardName),
        datasets: [
          {
            label: 'Number of Collections',
            data: rewardsData.map((data) => data.collectionCount),
            backgroundColor: gradientPurple, // Green
            borderColor: colors.fadedPeach.default,
            borderWidth: 2,
            hoverBackgroundColor: gradientPurple,
            borderRadius: 5,
          },
        ],
      },
      options: {
        indexAxis: 'y', // Makes the bar chart horizontal
        responsive: true,
        maintainAspectRatio: false,
        plugins: {
          datalabels: {
            display: true,
            color: 'white',
          },
          legend: {
            display: false, // Hide legend for simplicity
          },
          tooltip: {
            callbacks: {
              label: (tooltipItem) => {
                const value = tooltipItem.raw;
                return `Collections: ${value}`;
              },
            },
          },
        },
        scales: {
          x: {
            beginAtZero: true,
          },
          y: {
            // Customize the labels to show only 3 characters by default
            ticks: {
              callback: function (value) {
                const label = rewardsData[value].rewardName;
                return label.substring(0, 3); // Show full or shortened label
              },
            },
          },
        },
      },
    });
  }
}
