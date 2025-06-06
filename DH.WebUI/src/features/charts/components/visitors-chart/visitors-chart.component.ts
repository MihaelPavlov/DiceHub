import { StatisticsService } from './../../../../entities/statistics/api/statistics.service';
import {
  AfterViewInit,
  Component,
  ElementRef,
  OnDestroy,
  ViewChild,
} from '@angular/core';
import { Router } from '@angular/router';
import { NAV_ITEM_LABELS } from '../../../../shared/models/nav-items-labels.const';
import { MenuTabsService } from '../../../../shared/services/menu-tabs.service';
import { FormControl } from '@angular/forms';
import { addDays, addMonths, addYears, format } from 'date-fns';
import { Chart, registerables } from 'chart.js';
import 'chartjs-adapter-date-fns';
import ChartDataLabels from 'chartjs-plugin-datalabels';
import { colors } from '../../consts/colors.const';
import { ChartActivityType } from '../../../../entities/statistics/enums/chart-activity-type.enum';
import { OperationResult } from '../../../../shared/models/operation-result.model';
import { GetActivityChartData } from '../../../../entities/statistics/models/activity-chart.model';
import { ToastService } from '../../../../shared/services/toast.service';
import { ToastType } from '../../../../shared/models/toast.model';
import { AppToastMessage } from '../../../../shared/components/toast/constants/app-toast-messages.constant';
import { IDropdown } from '../../../../shared/models/dropdown.model';

@Component({
  selector: 'visitors-chart',
  templateUrl: 'visitors-chart.component.html',
  styleUrl: 'visitors-chart.component.scss',
})
export class VisitorsChartComponent implements AfterViewInit, OnDestroy {
  @ViewChild('visitorActivityChartCanvas')
  private visitorActivityChartCanvas!: ElementRef<HTMLCanvasElement>;
  private visitorActivityChart!: Chart;
  public chartType: IDropdown[] = [];
  public visitorsChartType = new FormControl(0);
  public currentRangeStart: Date = this.getStartOfWeek();
  public currentRangeEnd: Date = addDays(this.currentRangeStart, 6);

  constructor(
    private readonly menuTabsService: MenuTabsService,
    private readonly toastService: ToastService,
    private readonly router: Router,
    private readonly statisticsService: StatisticsService
  ) {
    Chart.register(ChartDataLabels, ...registerables);

    this.chartType = Object.entries(ChartActivityType)
      .filter(([key, value]) => typeof value === 'number')
      .map(([key, value]) => ({ id: value as number, name: key }));

    this.menuTabsService.setActive(NAV_ITEM_LABELS.PROFILE);
    this.visitorsChartType.valueChanges.subscribe(() => this.resetDateRange());
  }

  public ngAfterViewInit(): void {
    this.createVisitorWeekActivityChartCanvas(colors);
  }

  public ngOnDestroy(): void {
    this.menuTabsService.resetData();
  }

  public backNavigateBtn(): void {
    this.router.navigateByUrl('profile');
  }

  public updateDateRange(direction: 'forward' | 'backward'): void {
    const selectedType = this.visitorsChartType.value;
    if (this.visitorActivityChart) {
      this.visitorActivityChart.destroy();
    }
    let adjustmentValue = 0;
    if (selectedType === ChartActivityType.Weekly)
      adjustmentValue = 7; // Weekly
    else if (selectedType === ChartActivityType.Monthly)
      adjustmentValue = 1; // Monthly
    else if (selectedType === ChartActivityType.Yearly) adjustmentValue = 1; // Yearly

    if (direction === 'backward') adjustmentValue *= -1;

    if (selectedType === ChartActivityType.Weekly) {
      this.currentRangeStart = addDays(this.currentRangeStart, adjustmentValue);
      this.currentRangeEnd = addDays(this.currentRangeEnd, adjustmentValue);

      this.createVisitorWeekActivityChartCanvas(colors);
    } else if (selectedType === ChartActivityType.Monthly) {
      this.currentRangeStart = addMonths(
        this.currentRangeStart,
        adjustmentValue
      );

      this.createVisitorMonthActivityChartCanvas(colors);
    } else if (selectedType === ChartActivityType.Yearly) {
      this.currentRangeStart = addYears(
        this.currentRangeStart,
        adjustmentValue
      );

      this.createVisitorYearlyActivityChartCanvas(colors);
    }
  }

  public getFormattedRange(): string {
    let date;
    const selectedType = this.visitorsChartType.value;

    if (selectedType === (ChartActivityType.Weekly as number)) {
      date = `${format(this.currentRangeStart, 'MMM dd yyyy')} - ${format(
        this.currentRangeEnd,
        'MMM dd yyyy'
      )}`;
    } else if (selectedType === ChartActivityType.Monthly) {
      date = `${format(this.currentRangeStart, 'MMM yyyy')}`;
    } else if (selectedType === ChartActivityType.Yearly) {
      date = `${format(this.currentRangeStart, 'yyyy')}`;
    }
    return date;
  }

  private createVisitorWeekActivityChartCanvas(colors): void {
    this.statisticsService
      .getActivityChartData(
        ChartActivityType.Weekly,
        this.currentRangeStart.toISOString(),
        this.currentRangeEnd.toISOString()
      )
      .subscribe({
        next: (operation: OperationResult<GetActivityChartData> | null) => {
          if (operation && operation.success && operation.relatedObject) {
            const activityDataByWeek = operation.relatedObject?.logs;

            const dailyData = this.aggregateDataByDay(activityDataByWeek);

            // Visitors by week or 10 days
            const ctx2 =
              this.visitorActivityChartCanvas.nativeElement.getContext('2d');
            if (ctx2) {
              ctx2.canvas.height = 100;

              let gradient = ctx2.createLinearGradient(0, 25, 0, 300);
              gradient.addColorStop(0, colors.blue.half);
              gradient.addColorStop(0.35, colors.blue.quarter);
              gradient.addColorStop(1, colors.blue.zero);
              this.visitorActivityChart = new Chart(ctx2, {
                type: 'line',
                data: {
                  labels: dailyData.labels, // X-axis labels for daily data
                  datasets: [
                    {
                      label: 'Daily Visitor Activity',
                      data: dailyData.values,
                      fill: true,
                      backgroundColor: gradient,
                      tension: 0.5,
                      borderColor: colors.blue.quarter,
                      pointBackgroundColor: colors.blue.default,
                    },
                  ],
                },
                options: {
                  maintainAspectRatio: false,
                  plugins: {
                    legend: {
                      display: false,
                    },
                    tooltip: {
                      callbacks: {
                        // Customize the tooltip
                        title: (tooltipItems) => {
                          const rawDate = tooltipItems[0]?.label || '';
                          const cleanedDate = rawDate
                            .replace('a.m.', 'AM')
                            .replace('p.m.', 'PM');
                          const date = new Date(cleanedDate);

                          // Format for display
                          const formattedDate = new Intl.DateTimeFormat(
                            'en-US',
                            {
                              year: 'numeric',
                              month: 'short',
                              day: 'numeric',
                            }
                          ).format(date);
                          return `Date: ${formattedDate}`;
                        },
                        label: (tooltipItem) => {
                          const value = tooltipItem.raw;

                          return `Activity Count: ${value}`;
                        },
                      },
                    },
                    datalabels: {
                      display: false,
                    },
                  },
                  responsive: true,

                  scales: {
                    x: {
                      grid: {
                        display: false,
                      },
                      type: 'time',
                      time: {
                        round: 'day', // Use 'day' for daily data
                      },
                    },
                    y: {
                      max:
                        Math.max(
                          ...activityDataByWeek.map((item) => item.userCount)
                        ) + 5, // max should be bigger then the biggest userCount value
                      grid: {
                        color: '#31313b',
                      },
                      border: {
                        color: '#31313b',
                      },
                    },
                  },
                },
              });
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

  private createVisitorMonthActivityChartCanvas(colors): void {
    this.statisticsService
      .getActivityChartData(
        ChartActivityType.Monthly,
        this.currentRangeStart.toISOString()
      )
      .subscribe({
        next: (operation: OperationResult<GetActivityChartData> | null) => {
          if (operation && operation.success && operation.relatedObject) {
            const activityDataByDay = operation.relatedObject?.logs;
            const dailyData = this.aggregateDataByDay(activityDataByDay);

            // Visitors by week or 10 days
            const ctx2 =
              this.visitorActivityChartCanvas.nativeElement.getContext('2d');
            if (ctx2) {
              ctx2.canvas.height = 100;

              let gradient = ctx2.createLinearGradient(0, 25, 0, 300);
              gradient.addColorStop(0, colors.yellow.half);
              gradient.addColorStop(0.35, colors.yellow.quarter);
              gradient.addColorStop(1, colors.yellow.zero);
              this.visitorActivityChart = new Chart(ctx2, {
                type: 'line',
                data: {
                  labels: dailyData.labels, // X-axis labels for daily data
                  datasets: [
                    {
                      label: 'Daily Visitor Activity',
                      data: dailyData.values,
                      fill: true,
                      backgroundColor: gradient,
                      tension: 0.5,
                      borderColor: colors.yellow.quarter,
                      pointBackgroundColor: colors.yellow.default,
                    },
                  ],
                },
                options: {
                  maintainAspectRatio: false,
                  plugins: {
                    legend: {
                      display: false,
                    },
                    tooltip: {
                      callbacks: {
                        // Customize the tooltip
                        title: (tooltipItems) => {
                          const rawDate = tooltipItems[0]?.label || '';
                          const cleanedDate = rawDate
                            .replace('a.m.', 'AM')
                            .replace('p.m.', 'PM');
                          const date = new Date(cleanedDate);

                          // Format for display
                          const formattedDate = new Intl.DateTimeFormat(
                            'en-US',
                            {
                              year: 'numeric',
                              month: 'short',
                              day: 'numeric',
                            }
                          ).format(date);
                          return `Date: ${formattedDate}`;
                        },
                        label: (tooltipItem) => {
                          const value = tooltipItem.raw;

                          return `Activity Count: ${value}`;
                        },
                      },
                    },
                    datalabels: {
                      display: false,
                    },
                  },
                  responsive: true,

                  scales: {
                    x: {
                      bounds: 'ticks',
                      grid: {
                        display: false,
                      },
                      type: 'time',
                      time: {
                        round: 'day', // Use 'day' for daily data
                      },
                      ticks: {
                        // autoSkip: false,
                        maxTicksLimit: activityDataByDay.length / 2,
                        callback: (value, index, ticks) => {
                          const date = new Date(value);

                          return date.getDate(); // Show the day of the month
                        },
                      },
                    },
                    y: {
                      max:
                        Math.max(
                          ...activityDataByDay.map((item) => item.userCount)
                        ) + 5, // max should be bigger then the biggest userCount value
                      grid: {
                        color: '#31313b',
                      },
                      border: {
                        color: '#31313b',
                      },
                    },
                  },
                },
              });
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

  private createVisitorYearlyActivityChartCanvas(colors): void {
    this.statisticsService
      .getActivityChartData(
        ChartActivityType.Yearly,
        this.currentRangeStart.toISOString()
      )
      .subscribe({
        next: (operation: OperationResult<GetActivityChartData> | null) => {
          if (operation && operation.success && operation.relatedObject) {
            const activityDataByMonths = operation.relatedObject?.logs;

            const monthlyData = this.aggregateDataByMonth(activityDataByMonths);

            if (this.visitorActivityChartCanvas) {
              const ctx1 =
                this.visitorActivityChartCanvas.nativeElement.getContext('2d');
              // Monthly visitor activity
              if (ctx1) {
                ctx1.canvas.height = 100;

                let gradient = ctx1.createLinearGradient(0, 25, 0, 300);
                gradient.addColorStop(0, colors.green.half);
                gradient.addColorStop(0.35, colors.green.quarter);
                gradient.addColorStop(1, colors.green.zero);
                this.visitorActivityChart = new Chart(ctx1, {
                  type: 'line',
                  data: {
                    labels: monthlyData.labels, // X-axis labels for daily data
                    datasets: [
                      {
                        label: 'Daily Visitor Activity',
                        data: monthlyData.values,
                        fill: true,
                        backgroundColor: gradient,
                        borderColor: colors.green.quarter,
                        // borderColor: gradient,
                        // hoverBorderColor:gradient,
                        tension: 0.5,
                        pointBackgroundColor: colors.green.default,
                      },
                    ],
                  },
                  options: {
                    maintainAspectRatio: false,
                    plugins: {
                      legend: {
                        display: false,
                      },
                      tooltip: {
                        callbacks: {
                          // Customize the tooltip
                          title: (tooltipItems) => {
                            const rawDate = tooltipItems[0]?.label || '';
                            const cleanedDate = rawDate
                              .replace('a.m.', 'AM')
                              .replace('p.m.', 'PM');
                            const date = new Date(cleanedDate);

                            // Format for display
                            const formattedDate = new Intl.DateTimeFormat(
                              'en-US',
                              {
                                year: 'numeric',
                                month: 'short',
                                day: 'numeric',
                              }
                            ).format(date);
                            return `Date: ${formattedDate}`;
                          },
                          label: (tooltipItem) => {
                            const value = tooltipItem.raw;

                            return `Activity Count: ${value}`;
                          },
                        },
                      },
                      datalabels: {
                        display: false,
                      },
                    },
                    responsive: true,
                    scales: {
                      x: {
                        grid: {
                          display: false,
                          color: '#31313b',
                        },
                        type: 'time',
                        time: {
                          unit: 'month',
                          displayFormats: {
                            month: 'MMM',
                          },
                        },
                      },
                      y: {
                        max:
                          Math.max(
                            ...activityDataByMonths.map(
                              (item) => item.userCount
                            )
                          ) + 5, // max should be bigger then the biggest userCount value
                        grid: {
                          color: '#31313b',
                        },
                        border: {
                          color: '#31313b',
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

  private aggregateDataByDay(data: any[]): { labels: any[]; values: number[] } {
    const timeCounts: { [key: string]: number } = {};

    data.forEach((log) => {
      const date = log.date.split('T')[0];
      if (!timeCounts[date]) {
        timeCounts[date] = log.userCount;
      }
    });

    const labels = Object.keys(timeCounts).map((date) => new Date(date));
    const values = Object.values(timeCounts).map((set) => set);

    return { labels, values };
  }

  private aggregateDataByMonth(data: any[]): {
    labels: any[];
    values: number[];
  } {
    const timeCounts: { [key: string]: number } = {};

    // Count occurrences for each month
    data.forEach((log) => {
      const month = log.date.slice(0, 7);

      if (!timeCounts[month]) {
        timeCounts[month] = log.userCount;
      }
    });

    const labels = Object.keys(timeCounts).map((month) => month);
    const values = Object.values(timeCounts).map((set) => set);

    return { labels, values };
  }

  private resetDateRange(): void {
    const selectedType = this.visitorsChartType.value;

    if (this.visitorActivityChart) {
      this.visitorActivityChart.destroy();
    }
    if (selectedType === ChartActivityType.Weekly) {
      this.currentRangeStart = this.getStartOfWeek();
      this.currentRangeEnd = addDays(this.currentRangeStart, 6);

      this.createVisitorWeekActivityChartCanvas(colors);
    } else if (selectedType === ChartActivityType.Monthly) {
      this.currentRangeStart = new Date();
      this.currentRangeEnd = addMonths(this.currentRangeStart, 1);

      this.createVisitorMonthActivityChartCanvas(colors);
    } else if (selectedType === ChartActivityType.Yearly) {
      this.currentRangeStart = new Date();
      this.currentRangeEnd = addYears(this.currentRangeStart, 1);

      this.createVisitorYearlyActivityChartCanvas(colors);
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
