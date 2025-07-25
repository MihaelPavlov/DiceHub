import { map } from 'rxjs';
import { StatisticsService } from './../../../../../entities/statistics/api/statistics.service';
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
import { MenuTabsService } from '../../../../../shared/services/menu-tabs.service';
import { colors } from '../../../consts/colors.const';
import { addYears, format } from 'date-fns';
import { ToastService } from '../../../../../shared/services/toast.service';
import { AppToastMessage } from '../../../../../shared/components/toast/constants/app-toast-messages.constant';
import { ToastType } from '../../../../../shared/models/toast.model';
import { RewardsStats } from '../../../../../entities/statistics/models/expired-collected-rewards-chart.model';

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
  public currentRangeStart: Date = new Date();
  allMonths = [
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
  ];

  public selectedMonths: string[] = [];
  collectedRewards: RewardsStats[] = [];
  uncollectedRewards: RewardsStats[] = [];

  filteredCollected: RewardsStats[] = [];
  filteredUncollected: RewardsStats[] = [];
  constructor(
    private readonly router: Router,
    private readonly menuTabsService: MenuTabsService,
    private readonly toastService: ToastService,
    private readonly statisticsService: StatisticsService
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

  public updateDateRange(direction: 'forward' | 'backward'): void {
    if (this.rewardsChart) {
      this.rewardsChart.destroy();
    }
    let adjustmentValue = 1;

    if (direction === 'backward') adjustmentValue *= -1;

    this.currentRangeStart = addYears(this.currentRangeStart, adjustmentValue);

    this.createRewardsChartCanvas(colors);
  }

  public getFormattedRange(): string {
    return `${format(this.currentRangeStart, 'yyyy')}`;
  }

  public createRewardsChartCanvas(colors): void {
    if (this.rewardsChart) {
      this.rewardsChart.destroy();
    }

    this.statisticsService
      .getExpiredCollectedRewardChartData(this.currentRangeStart.getFullYear())
      .subscribe({
        next: (operation) => {
          if (operation && operation.success && operation.relatedObject) {
            this.collectedRewards = operation.relatedObject.collected;
            this.uncollectedRewards = operation.relatedObject.expired;

            if (this.rewardsChartCanvas) {
              const ctx4 =
                this.rewardsChartCanvas.nativeElement.getContext('2d');
              if (ctx4) {
                let gradientPeach = ctx4.createLinearGradient(0, 25, 50, 300);
                gradientPeach.addColorStop(0, colors.peach.half);

                let gradientPurple = ctx4.createLinearGradient(0, 25, 50, 300);
                gradientPurple.addColorStop(0, colors.purple2.half);

                const { labels, collected, uncollected } =
                  this.getFilteredChartData();

                this.rewardsChart = new Chart(ctx4, {
                  type: 'bar',
                  data: {
                    labels: labels,
                    datasets: [
                      {
                        barThickness: 15,
                        label: 'Collected Rewards',
                        data: collected,
                        backgroundColor: gradientPeach, // Green
                        borderColor: colors.peach.default,
                        borderWidth: 2,
                        hoverBackgroundColor: gradientPeach,
                        borderRadius: 5,
                      },
                      {
                        barThickness: 15,
                        label: 'Expired Rewards',
                        data: uncollected,
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
                        display: true,
                        anchor: 'center', // attach to the top of the bar
                        align: 'center', // place label above the bar
                        color: '#ffffff',
                        font: {
                          weight: 'bold',
                          size: 12,
                        },
                        formatter: (value, context) => {
                          const index = context.dataIndex;
                          const datasetIndex = context.datasetIndex;
                          const data =
                            datasetIndex === 0
                              ? this.filteredCollected
                              : this.filteredUncollected;
                          const item = data[index];
                          return item && item.countRewards > 0
                            ? `${
                                item.countRewards
                              } / ${item.countRewards.toFixed(2)} BGN`
                            : '';
                        },
                      },
                      tooltip: {
                        callbacks: {
                          label: (tooltipItem) => {
                            const index = tooltipItem.dataIndex;
                            const datasetIndex = tooltipItem.datasetIndex;
                            const isCollected = datasetIndex === 0;
                            const data = isCollected
                              ? this.filteredCollected
                              : this.filteredUncollected;
                            const item = data[index];
                            return item
                              ? `Count: ${
                                  item.countRewards
                                } | Cash: ${item.countRewards.toFixed(2)} BGN`
                              : '';
                          },
                        },
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
        },
        error: () => {
          this.toastService.error({
            message: AppToastMessage.SomethingWrong,
            type: ToastType.Error,
          });
        },
      });
  }

  private getFilteredChartData(): {
    labels: string[];
    collected: number[];
    uncollected: number[];
  } {
    const labels: string[] = [];
    const collected: number[] = [];
    const uncollected: number[] = [];

    this.filteredCollected = [];
    this.filteredUncollected = [];

    this.allMonths.forEach((month, index) => {
      const shouldInclude =
        !this.selectedMonths?.length || this.selectedMonths.includes(month);

      if (shouldInclude) {
        const collectedItem = this.collectedRewards[index] || {
          countRewards: 0,
        };
        const uncollectedItem = this.uncollectedRewards[index] || {
          countRewards: 0,
        };

        labels.push(month);
        collected.push(collectedItem.countRewards);
        uncollected.push(uncollectedItem.countRewards);

        this.filteredCollected.push(collectedItem);
        this.filteredUncollected.push(uncollectedItem);
      }
    });

    return { labels, collected, uncollected };
  }

  public updateChartByMonth(): void {
    if (!this.collectedRewards.length || !this.uncollectedRewards.length)
      return;

    const { labels, collected, uncollected } = this.getFilteredChartData();
    this.rewardsChart.data.labels = labels;
    this.rewardsChart.data.datasets[0].data = collected;
    this.rewardsChart.data.datasets[1].data = uncollected;

    this.rewardsChart.update();
  }
}
