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
import { DatePipe } from '@angular/common';

@Component({
    selector: 'visitors-chart',
    templateUrl: 'visitors-chart.component.html',
    styleUrl: 'visitors-chart.component.scss',
    standalone: false
})
export class VisitorsChartComponent implements AfterViewInit, OnDestroy {
  @ViewChild('visitorActivityChartCanvas')
  private visitorActivityChartCanvas!: ElementRef<HTMLCanvasElement>;
  private visitorActivityChart!: Chart;
  public chartType: IDropdown[] = [];
  public visitorsChartType = new FormControl<number | null>(0);
  public currentRangeStart: Date = this.getStartOfWeek();
  public currentRangeEnd: Date = addDays(this.currentRangeStart, 6);
  public isVisitorDataAvailable = false;
  public readonly ChartActivityType = ChartActivityType;
  constructor(
    private readonly menuTabsService: MenuTabsService,
    private readonly toastService: ToastService,
    private readonly router: Router,
    private readonly statisticsService: StatisticsService,
    private datePipe: DatePipe
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
    const selectedType = this.visitorsChartType
      .value as ChartActivityType | null;
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
        this.currentRangeStart,
        this.currentRangeEnd
      )
      .subscribe({
        next: (operation: OperationResult<GetActivityChartData> | null) => {
          if (operation && operation.success && operation.relatedObject) {
            const activityDataByWeek = operation.relatedObject?.logs;

            const dailyData = this.aggregateDataByDay(activityDataByWeek);

            // Visitors by week
            const ctx2 =
              this.visitorActivityChartCanvas.nativeElement.getContext('2d');
            if (ctx2 && this.isVisitorDataAvailable) {
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
                        unit: 'day',
                        tooltipFormat: 'MMM dd yyyy',
                        displayFormats: {
                          day: 'MMM dd', // e.g., "Jul 28"
                        },
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
      .getActivityChartData(ChartActivityType.Monthly, this.currentRangeStart)
      .subscribe({
        next: (operation: OperationResult<GetActivityChartData> | null) => {
          if (operation && operation.success && operation.relatedObject) {
            const activityDataByDay = operation.relatedObject.logs;
            const dailyData = this.aggregateDataByDay(activityDataByDay);

            const ctx =
              this.visitorActivityChartCanvas.nativeElement.getContext('2d');
            if (
              ctx &&
              this.visitorActivityChartCanvas.nativeElement.parentElement &&
              this.isVisitorDataAvailable
            ) {
              const barHeight = 30; // Customize this value
              const minHeight = 300; // Minimum height to ensure reasonable display

              // Dynamically calculate the container height based on data length
              const computedHeight = Math.max(
                dailyData.labels.length * barHeight,
                minHeight
              );

              this.visitorActivityChartCanvas.nativeElement.parentElement.style.height = `${computedHeight}px`;
              let gradient = ctx.createLinearGradient(0, 25, 0, 300);
              gradient.addColorStop(0, colors.yellow.half);
              gradient.addColorStop(0.35, colors.yellow.default);
              gradient.addColorStop(1, colors.yellow.zero);
              this.visitorActivityChart = new Chart(ctx, {
                type: 'bar',
                data: {
                  labels: dailyData.labels,
                  datasets: [
                    {
                      label: 'Visitor Activity',
                      data: dailyData.values.map((v) => (v === 0 ? null : v)),
                      backgroundColor: colors.yellow.half,
                      borderColor: colors.yellow.default,
                      borderWidth: 2,
                      hoverBackgroundColor: colors.yellow.quarter,
                      borderRadius: 5,
                      barThickness: 15,
                      barPercentage: 30,
                    },
                  ],
                },
                options: {
                  indexAxis: 'y',
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
                    },
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
                  },
                  scales: {
                    x: {
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
                    y: {
                      type: 'time',
                      reverse: true,
                      time: {
                        unit: 'day',
                        tooltipFormat: 'MMM dd yyyy',
                        displayFormats: {
                          day: 'MMM dd',
                        },
                      },
                      grid: {
                        display: false,
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
      .getActivityChartData(ChartActivityType.Yearly, this.currentRangeStart)
      .subscribe({
        next: (operation: OperationResult<GetActivityChartData> | null) => {
          if (operation && operation.success && operation.relatedObject) {
            const activityDataByMonths = operation.relatedObject?.logs;

            const monthlyData = this.aggregateDataByMonth(activityDataByMonths);

            if (this.visitorActivityChartCanvas) {
              const ctx1 =
                this.visitorActivityChartCanvas.nativeElement.getContext('2d');
              // Monthly visitor activity
              if (ctx1 && this.isVisitorDataAvailable) {
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
    this.isVisitorDataAvailable = false;
    const timeCounts: { [key: string]: number } = {};

    // Step 1: Parse and normalize each log's date
    data.forEach((log) => {
      const utcDate = new Date(log.date);
      const localDate = new Date(
        utcDate.getFullYear(),
        utcDate.getMonth(),
        utcDate.getDate()
      ); // local midnight
      const dateKey = localDate.toDateString(); // e.g. "Mon Jul 28 2025"
      timeCounts[dateKey] = log.userCount;
    });
    // Determine range
    let start: Date;
    let end: Date;

    if (this.visitorsChartType.value === ChartActivityType.Monthly) {
      // Use range from backend data
      const dates = data.map((log) => new Date(log.date));
      dates.sort((a, b) => a.getTime() - b.getTime());
      start = new Date(
        dates[0].getFullYear(),
        dates[0].getMonth(),
        dates[0].getDate()
      );
      end = new Date(
        dates[dates.length - 1].getFullYear(),
        dates[dates.length - 1].getMonth(),
        dates[dates.length - 1].getDate()
      );
    } else {
      // Use predefined range
      start = new Date(
        this.currentRangeStart.getFullYear(),
        this.currentRangeStart.getMonth(),
        this.currentRangeStart.getDate()
      );
      end = new Date(
        this.currentRangeEnd.getFullYear(),
        this.currentRangeEnd.getMonth(),
        this.currentRangeEnd.getDate()
      );
    }
    const labels: Date[] = [];
    const values: number[] = [];

    const current = new Date(start);

    while (current <= end) {
      const key = current.toDateString(); // same format as above
      labels.push(new Date(current)); // push a copy
      values.push(timeCounts[key] ?? 0); // fill 0 if missing
      current.setDate(current.getDate() + 1);
    }

    if (values.find((v) => v > 0)) this.isVisitorDataAvailable = true;

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

    if (values.find((v) => v > 0)) this.isVisitorDataAvailable = true;
    return { labels, values };
  }

  private resetDateRange(): void {
    const selectedType = this.visitorsChartType
      .value as ChartActivityType | null;

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
      this.currentRangeStart = new Date(new Date().getFullYear(), 1, 1);
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
