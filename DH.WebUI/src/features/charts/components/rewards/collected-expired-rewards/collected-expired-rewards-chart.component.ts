import {
  Component,
  AfterViewInit,
  OnDestroy,
  ViewChild,
  ElementRef,
} from '@angular/core';
import { Router } from '@angular/router';
import { Chart, registerables } from 'chart.js';
import ChartDataLabels from 'chartjs-plugin-datalabels';
import { NAV_ITEM_LABELS } from '../../../../../shared/models/nav-items-labels.const';
import { LoadingService } from '../../../../../shared/services/loading.service';
import { MenuTabsService } from '../../../../../shared/services/menu-tabs.service';
import { colors } from '../../../consts/colors.const';
import { addDays, addYears, format } from 'date-fns';

@Component({
  selector: 'collected-expired-rewards-chart',
  templateUrl: 'collected-expired-rewards-chart.component.html',
  styleUrl: 'collected-expired-rewards-chart.component.scss',
})
export class CollectedExpiredRewardsChartComponent
  implements AfterViewInit, OnDestroy
{
  @ViewChild('rewardsChartCanvas')
  private rewardsChartCanvas!: ElementRef<HTMLCanvasElement>;
  private rewardsChart: any;
  public isMenuVisible: boolean = false;
  public currentRangeStart: Date = new Date();
  public currentRangeEnd: Date = addDays(this.currentRangeStart, 7);
  constructor(
    private readonly loadingService: LoadingService,
    private readonly router: Router,
    private readonly menuTabsService: MenuTabsService
  ) {
    this.menuTabsService.setActive(NAV_ITEM_LABELS.PROFILE);

    Chart.register(ChartDataLabels, ...registerables);
  }

  public ngOnDestroy(): void {
    this.menuTabsService.resetData();
  }
  public ngAfterViewInit(): void {
    this.createRewardsChartCanvas(colors);
  }
  public backNavigateBtn(): void {
    this.router.navigateByUrl('charts/rewards');
  }
  public showMenu(): void {
    this.isMenuVisible = !this.isMenuVisible;
  }

  public updateDateRange(direction: 'forward' | 'backward'): void {
    this.loadingService.loadingOn();
    if (this.rewardsChart) {
      this.rewardsChart.destroy();
    }
    let adjustmentValue = 1;

    if (direction === 'backward') adjustmentValue *= -1;

    this.currentRangeStart = addYears(this.currentRangeStart, adjustmentValue);

    this.createRewardsChartCanvas(colors);

    this.loadingService.loadingOff();
  }

  public getFormattedRange(): string {
    return `${format(this.currentRangeStart, 'yyyy')}`;
  }

  public createRewardsChartCanvas(colors): void {
    const ctx4 = this.rewardsChartCanvas.nativeElement.getContext('2d');

    const collectedRewards = [10, 20, 15, 30, 25, 40, 35, 50, 45, 55, 60, 70];
    const uncollectedRewards = [5, 15, 10, 20, 10, 25, 20, 30, 25, 40, 30, 35];
    if (ctx4) {
      let gradientPeach = ctx4.createLinearGradient(0, 25, 50, 300);
      gradientPeach.addColorStop(0, colors.peach.half);

      let gradientPurple = ctx4.createLinearGradient(0, 25, 50, 300);
      gradientPurple.addColorStop(0, colors.purple2.half);

      this.rewardsChart = new Chart(ctx4, {
        type: 'bar',
        data: {
          // TODO: Generate these labels based on the year and how many records we have for each month
          // if we don't have for specific month we don't visualize the specific month
          labels: [
            'Jan',
            'Feb',
            'Mar',
            'Apr',
            'May',
            'Jun',
            'Jul',
            'Aug',
            'Sep',
            'Oct',
            'Nov',
            'Dec',
          ],
          datasets: [
            {
              barThickness: 15,
              label: 'Collected Rewards',
              data: collectedRewards,
              backgroundColor: gradientPeach, // Green
              borderColor: colors.peach.default,
              borderWidth: 2,
              hoverBackgroundColor: gradientPeach,
              borderRadius: 5,
            },
            {
              barThickness: 15,
              label: 'Expired Rewards',
              data: uncollectedRewards,
              backgroundColor: gradientPurple, // Purple
              borderColor: colors.purple2.default,
              borderWidth: 1,
              hoverBackgroundColor: gradientPurple,
              borderRadius: 5,
            },
          ],
        },
        options: {
          responsive: true,
          maintainAspectRatio: false,
          indexAxis: 'y', // Flip the chart orientation
          plugins: {
            legend: {
              display: true,
              position: 'top',
            },
            datalabels: {
              display: false,
            },
          },
          scales: {
            x: {
              beginAtZero: true,
              title: {
                display: true,
                text: 'Rewards Count',
              },
              grid: {
                color: '#31313b',
              },
            },
            y: {
              grid: {
                display: false,
              },
            },
          },
        },
        plugins: [
          {
            id: 'customLegendPadding',
            beforeInit(chart) {
              const originalFit = chart.legend?.fit;
              chart.legend!.fit = function fit() {
                originalFit!.bind(chart.legend)();
                this.height += 20;
              };
            },
          },
        ],
      });
    }
  }
}
