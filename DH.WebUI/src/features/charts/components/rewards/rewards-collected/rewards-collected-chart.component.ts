import { Component, OnDestroy, ViewChild, ElementRef } from '@angular/core';
import ChartDataLabels from 'chartjs-plugin-datalabels';
import { Router } from '@angular/router';
import { Chart, registerables } from 'chart.js';
import { NAV_ITEM_LABELS } from '../../../../../shared/models/nav-items-labels.const';
import { MenuTabsService } from '../../../../../shared/services/menu-tabs.service';
import { colors } from '../../../consts/colors.const';
import { FormControl } from '@angular/forms';
import { RewardsService } from '../../../../../entities/rewards/api/rewards.service';
import { StatisticsService } from '../../../../../entities/statistics/api/statistics.service';
import { ToastService } from '../../../../../shared/services/toast.service';
import { combineLatest } from 'rxjs';
import { AppToastMessage } from '../../../../../shared/components/toast/constants/app-toast-messages.constant';
import { ToastType } from '../../../../../shared/models/toast.model';

@Component({
  selector: 'rewards-collected-chart',
  templateUrl: 'rewards-collected-chart.component.html',
  styleUrl: 'rewards-collected-chart.component.scss',
})
export class RewardsCollectedChartComponent implements OnDestroy {
  @ViewChild('rewardsChartCanvas')
  private rewardsChartCanvas!: ElementRef<HTMLCanvasElement>;
  private rewardsChart!: Chart;

  private REQUIRED_MESSAGE_FROM_DATES: string =
    'Specifying from which date we start is required.';

  public fromDateControl = new FormControl(null);
  public toDateControl = new FormControl(null);
  public validationMessageFromDates: string | null =
    this.REQUIRED_MESSAGE_FROM_DATES;

  constructor(
    private readonly router: Router,
    private readonly menuTabsService: MenuTabsService,
    private readonly toastService: ToastService,
    private readonly statisticsService: StatisticsService,
    private readonly rewardsService: RewardsService
  ) {
    this.menuTabsService.setActive(NAV_ITEM_LABELS.PROFILE);

    Chart.register(ChartDataLabels, ...registerables);
    this.fromDateControl.valueChanges.subscribe(() => this.dateValidation());
    this.toDateControl.valueChanges.subscribe(() => this.dateValidation());
  }

  public dateValidation(): void {
    if (this.rewardsChart) {
      this.rewardsChart.destroy();
    }

    if (this.toDateControl.value) {
      if (!this.fromDateControl.value) {
        this.validationMessageFromDates = this.REQUIRED_MESSAGE_FROM_DATES;
        return;
      } else if (this.fromDateControl.value >= this.toDateControl.value) {
        this.validationMessageFromDates =
          'The from-date must be earlier than the to-date.';
        return;
      }
    }

    if (!this.fromDateControl.value) {
      this.validationMessageFromDates = this.REQUIRED_MESSAGE_FROM_DATES;
      return;
    }

    this.validationMessageFromDates = null;

    this.createRewardsStatsChartCanvas(colors);
  }

  public ngOnDestroy(): void {
    this.menuTabsService.resetData();
  }

  public backNavigateBtn(): void {
    this.router.navigateByUrl('charts/rewards');
  }

  public createRewardsStatsChartCanvas(colors): void {
    if (this.fromDateControl.value && this.toDateControl.value) {
      combineLatest([
        this.rewardsService.getAllSystemRewards(),
        this.statisticsService.getCollectedRewardsByDates(
          new Date(this.fromDateControl.value).toISOString(),
          new Date(this.toDateControl.value).toISOString()
        ),
      ]).subscribe({
        next: ([rewards, operation]) => {
          if (
            rewards &&
            operation &&
            operation.success &&
            operation.relatedObject
          ) {
            const rewardsData = operation.relatedObject.map((x) => ({
              collectionCount: x.collectedCount,
              totalCashEquivalent: x.totalCashEquivalent,
              rewardName:
                rewards.find((e) => e.id == x.rewardId)?.name ||
                'Reward Deleted',
            }));

            if (this.rewardsChartCanvas) {
              const ctx =
                this.rewardsChartCanvas.nativeElement.getContext('2d');
              if (ctx) {
                const barHeight = 30; // Customize this value
                const minHeight = 300; // Minimum height to ensure reasonable display

                // Dynamically calculate the container height based on data length
                const computedHeight = Math.max(
                  rewardsData.length * barHeight,
                  minHeight
                );

                // Apply the height dynamically
                if (this.rewardsChartCanvas.nativeElement.parentElement)
                  this.rewardsChartCanvas.nativeElement.parentElement.style.height = `${computedHeight}px`;

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
                        barThickness: 15,
                        barPercentage: 30,
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
                        font: {
                          weight: 'bold',
                          size: 12,
                        },
                        color: 'white',
                        formatter: function (value, context) {
                          const index = context.dataIndex;
                          const reward = rewardsData[index];
                          return `${reward.collectionCount} / ${reward.totalCashEquivalent.toFixed(2)} BGN`;
                        },
                      },
                      legend: {
                        display: false, // Hide legend for simplicity
                      },
                      tooltip: {
                        callbacks: {
                          label: (tooltipItem) => {
                            const index = tooltipItem.dataIndex;
                            const reward = rewardsData[index];
                            return `Collections: ${
                              reward.collectionCount
                            } | Total Cash: ${reward.totalCashEquivalent.toFixed(
                              2
                            )} BGN`;
                          },
                        },
                      },
                    },
                    scales: {
                      x: {
                        beginAtZero: true,
                      },
                      y: {
                        // Customize the labels to show only 8 characters by default
                        ticks: {
                          callback: function (value) {
                            const label = rewardsData[value].rewardName;
                            if (label.length > 8)
                              return label.substring(0, 8) + '..';
                            return label;
                          },
                        },
                      },
                    },
                  },
                });
              }
            }
          }
        },
        error: () => {
          this.toastService.error({
            message: AppToastMessage.SomethingWrong,
            type: ToastType.Error,
          });
        },
      });
    }
  }
}
