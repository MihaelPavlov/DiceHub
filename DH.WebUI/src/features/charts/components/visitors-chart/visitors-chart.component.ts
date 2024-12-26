import { timer } from 'rxjs';
import { AfterViewInit, Component, ElementRef, ViewChild } from '@angular/core';
import { Router } from '@angular/router';
import { NAV_ITEM_LABELS } from '../../../../shared/models/nav-items-labels.const';
import { MenuTabsService } from '../../../../shared/services/menu-tabs.service';
import { FormControl } from '@angular/forms';
import { addDays, addMonths, addYears, format } from 'date-fns';
import { LoadingService } from '../../../../shared/services/loading.service';
import { Chart, registerables } from 'chart.js';
import 'chartjs-adapter-date-fns';
import ChartDataLabels from 'chartjs-plugin-datalabels';
import { colors } from '../../consts/colors.const';

interface IDropdown {
  id: number;
  name: string;
}

@Component({
  selector: 'visitors-chart',
  templateUrl: 'visitors-chart.component.html',
  styleUrl: 'visitors-chart.component.scss',
})
export class VisitorsChartComponent  implements AfterViewInit{
  @ViewChild('visitorActivityChartCanvas')
  private visitorActivityChartCanvas!: ElementRef<HTMLCanvasElement>;
  public visitorActivityChart: any;
  public chartType: IDropdown[] = [
    {
      id: 1,
      name: 'Weekly',
    },
    {
      id: 2,
      name: 'Monthly',
    },
    {
      id: 3,
      name: 'Yearly',
    },
  ];
  public isMenuVisible: boolean = false;
  public visitorsChartType = new FormControl(1);
  public currentRangeStart: Date = new Date();
  public currentRangeEnd: Date = addDays(this.currentRangeStart, 7);

  constructor(
    private readonly menuTabsService: MenuTabsService,
    private readonly loadingService: LoadingService,
    private readonly router: Router
  ) {
    Chart.register(ChartDataLabels, ...registerables);
    this.menuTabsService.setActive(NAV_ITEM_LABELS.PROFILE);
    this.visitorsChartType.valueChanges.subscribe(() => this.resetDateRange());
  }
  public ngAfterViewInit(): void {
    this.createVisitorWeekActivityChartCanvas(colors);
  }

  public backNavigateBtn(): void {
    this.router.navigateByUrl('profile');
  }

  public showMenu(): void {
    this.isMenuVisible = !this.isMenuVisible;
  }

  public createVisitorWeekActivityChartCanvas(colors): void {
    // Hardcoded visitor activity data
    const activityDataByWeek = [
      { userCount: 1, dateTime: '2024-12-21T10:30:00Z' },
      { userCount: 2, dateTime: '2024-12-22T10:45:00Z' },
      { userCount: 1, dateTime: '2024-12-23T11:00:00Z' },
      { userCount: 2, dateTime: '2024-12-24T11:30:00Z' },
      { userCount: 3, dateTime: '2024-12-25T11:15:00Z' },
      { userCount: 3, dateTime: '2024-12-26T12:00:00Z' },
      { userCount: 30, dateTime: '2024-12-27T12:15:00Z' },
      { userCount: 1, dateTime: '2024-12-28T11:45:00Z' },
      { userCount: 5, dateTime: '2024-12-29T12:00:00Z' }, // Visit in December (month)
      { userCount: 1000, dateTime: '2024-12-30T12:30:00Z' }, // Visit in December (month)
    ];
    console.log(this.visitorActivityChartCanvas);

    const dailyData = this.aggregateDataByDay(activityDataByWeek);

    // Visitors by week or 10 days
    const ctx2 = this.visitorActivityChartCanvas.nativeElement.getContext('2d');
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
                  const formattedDate = new Intl.DateTimeFormat('en-US', {
                    year: 'numeric',
                    month: 'short',
                    day: 'numeric',
                  }).format(date);
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
              // ticks: {
              //   callback: (value, index, ticks) => {
              //     const date = new Date(value);
              //     const totalTicks = ticks.length;
              //     console.log(index, '---------', totalTicks);
              //     if (index === totalTicks - 1) {
              //       console.log('less');

              //       return ''
              //     }

              //     if (index === 0 || index === totalTicks - 1) {
              //       return date.getDate(); // Show the day of the month
              //     }

              //     return date.getDate();
              //   },
              // },
            },
            y: {
              max:
                Math.max(...activityDataByWeek.map((item) => item.userCount)) +
                5, // max should be bigger then the biggest userCount value
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

  public createVisitorMonthActivityChartCanvas(colors): void {
    // Hardcoded visitor activity data
    const activityDataByWeek = [
      { userCount: 1, dateTime: '2024-12-01T10:30:00Z' },
      { userCount: 2, dateTime: '2024-12-02T10:45:00Z' },
      { userCount: 1, dateTime: '2024-12-03T11:00:00Z' },
      { userCount: 2, dateTime: '2024-12-04T11:30:00Z' },
      { userCount: 3, dateTime: '2024-12-05T11:15:00Z' },
      { userCount: 3, dateTime: '2024-12-06T12:00:00Z' },
      { userCount: 30, dateTime: '2024-12-07T12:15:00Z' },
      { userCount: 1, dateTime: '2024-12-08T11:45:00Z' },
      { userCount: 5, dateTime: '2024-12-09T12:00:00Z' }, // Visit in December (month)
      { userCount: 10, dateTime: '2024-12-10T12:30:00Z' }, // Visit in D
      { userCount: 1, dateTime: '2024-12-11T10:30:00Z' },
      { userCount: 2, dateTime: '2024-12-12T10:45:00Z' },
      { userCount: 1, dateTime: '2024-12-13T11:00:00Z' },
      { userCount: 2, dateTime: '2024-12-14T11:30:00Z' },
      { userCount: 3, dateTime: '2024-12-15T11:15:00Z' },
      { userCount: 3, dateTime: '2024-12-16T12:00:00Z' },
      { userCount: 30, dateTime: '2024-12-17T12:15:00Z' },
      { userCount: 1, dateTime: '2024-12-18T11:45:00Z' },
      { userCount: 5, dateTime: '2024-12-19T12:00:00Z' }, // Visit in December (month)
      { userCount: 10, dateTime: '2024-12-20T12:30:00Z' }, //
      { userCount: 1, dateTime: '2024-12-21T10:30:00Z' },
      { userCount: 2, dateTime: '2024-12-22T10:45:00Z' },
      { userCount: 1, dateTime: '2024-12-23T11:00:00Z' },
      { userCount: 2, dateTime: '2024-12-24T11:30:00Z' },
      { userCount: 3, dateTime: '2024-12-25T11:15:00Z' },
      { userCount: 3, dateTime: '2024-12-26T12:00:00Z' },
      { userCount: 30, dateTime: '2024-12-27T12:15:00Z' },
      { userCount: 1, dateTime: '2024-12-28T11:45:00Z' },
      { userCount: 5, dateTime: '2024-12-29T12:00:00Z' }, // Visit in December (month)
      { userCount: 10, dateTime: '2024-12-30T12:30:00Z' }, // Visit in December (month)
      { userCount: 10, dateTime: '2024-12-31T12:30:00Z' }, // Visit in December (month)
    ];
    console.log(this.visitorActivityChartCanvas);

    const dailyData = this.aggregateDataByDay(activityDataByWeek);

    // Visitors by week or 10 days
    const ctx2 = this.visitorActivityChartCanvas.nativeElement.getContext('2d');
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
              // pointBackgroundColor: '#52cba4', //'2a78af'
              // pointBorderWidth:2.5,
              // pointBorderColor:'#52cba4',
              tension: 0.5,
              borderColor: colors.yellow.quarter,
              pointBackgroundColor: colors.yellow.default,
            },
            // {
            //   label: 'Monthly Visitor Activity',
            //   data: monthlyData.values,
            //   fill: 'start',
            //   borderColor: '#52cba4', #52cba4
            //   tension: 0.5,
            // },
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
                  const formattedDate = new Intl.DateTimeFormat('en-US', {
                    year: 'numeric',
                    month: 'short',
                    day: 'numeric',
                  }).format(date);
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
            // datalabels: {
            //   display: true,
            //   align: 'top', // Position the labels
            //   color: '#75a0ff', // Label text color
            //   formatter: (value) => value, // Show the value
            // },
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
              // title: {
              //   display: true,
              //   text: 'Date/Time',
              // },
              //   TODO: This should be only for the month
              ticks: {
                // autoSkip: false,
                maxTicksLimit: activityDataByWeek.length / 2,
                callback: (value, index, ticks) => {
                  const date = new Date(value);
                  //     const totalTicks = ticks.length;
                  // console.log(index,'-----',totalTicks);

                  //     if (index === 0 || index === totalTicks - 1) {
                  //       return date.getDate(); // Show the day of the month
                  //     }

                  return date.getDate(); // Show the day of the month
                },
              },
            },
            y: {
              max:
                Math.max(...activityDataByWeek.map((item) => item.userCount)) +
                5, // max should be bigger then the biggest userCount value
              grid: {
                color: '#31313b',
              },
              border: {
                color: '#31313b',
              },
            },

            // y: {
            //   title: {
            //     display: true,
            //     text: 'Activity Count',
            //   },
            // },
          },
        },
      });
    }
  }

  public createVisitorYearlyActivityChartCanvas(colors): void {
    const activityDataByMonths = [
      { userCount: 1, dateTime: '2024-01-20T10:30:00Z' },
      { userCount: 1, dateTime: '2024-02-20T10:30:00Z' },
      { userCount: 22, dateTime: '2024-03-20T10:30:00Z' },
      { userCount: 2, dateTime: '2024-04-21T10:45:00Z' },
      { userCount: 1, dateTime: '2024-05-22T11:00:00Z' },
      { userCount: 2, dateTime: '2024-06-23T11:30:00Z' },
      { userCount: 3, dateTime: '2024-07-24T11:15:00Z' },
      { userCount: 3, dateTime: '2024-08-25T12:00:00Z' },
      { userCount: 30, dateTime: '2024-09-26T12:15:00Z' },
      { userCount: 1, dateTime: '2024-10-27T11:45:00Z' },
      { userCount: 5, dateTime: '2024-11-28T12:00:00Z' },
      { userCount: 10, dateTime: '2024-12-30T12:30:00Z' },
    ];
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
                    const formattedDate = new Intl.DateTimeFormat('en-US', {
                      year: 'numeric',
                      month: 'short',
                      day: 'numeric',
                    }).format(date);
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
                    ...activityDataByMonths.map((item) => item.userCount)
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

  private aggregateDataByDay(data: any[]): { labels: any[]; values: number[] } {
    const timeCounts: { [key: string]: number } = {}; // Use Set to count unique users

    // Count occurrences for each day, ensuring unique users
    data.forEach((log) => {
      const date = new Date(log.dateTime).toISOString().split('T')[0]; // Only keep date (no time)
      if (!timeCounts[date]) {
        timeCounts[date] = log.userCount;
      }
    });
    // Prepare chart data for daily visits

    const labels = Object.keys(timeCounts).map((date) => new Date(date));
    const values = Object.values(timeCounts).map((set) => set); // Count unique users

    return { labels, values };
  }

  private aggregateDataByMonth(data: any[]): {
    labels: any[];
    values: number[];
  } {
    const timeCounts: { [key: string]: number } = {}; // Use Set to count unique users

    // Count occurrences for each month
    data.forEach((log) => {
      const month = new Date(log.dateTime).toISOString().slice(0, 7); // Get YYYY-MM for monthly grouping

      if (!timeCounts[month]) {
        timeCounts[month] = log.userCount;
      }
    });

    // Prepare chart data for monthly visits
    const labels = Object.keys(timeCounts).map((month) => month); // Use the 1st day of the month for labels
    const values = Object.values(timeCounts).map((set) => set); // Count unique users

    return { labels, values };
  }

  public resetDateRange(): void {
    const selectedType = this.visitorsChartType.value;
    if (this.visitorActivityChart) {
      this.visitorActivityChart.destroy();
    }
    if (selectedType === 1) {
      // Weekly

      this.currentRangeStart = new Date();
      this.currentRangeEnd = addDays(this.currentRangeStart, 7);

      console.log('weekly -> ', this.currentRangeStart, this.currentRangeEnd);
      this.createVisitorWeekActivityChartCanvas(colors);
    } else if (selectedType === 2) {
      // Monthly
      this.currentRangeStart = new Date();
      this.currentRangeEnd = addMonths(this.currentRangeStart, 1);
      console.log('monthly -> ', this.currentRangeStart);
      this.createVisitorMonthActivityChartCanvas(colors);
    } else if (selectedType === 3) {
      // Yearly
      this.currentRangeStart = new Date();
      this.currentRangeEnd = addYears(this.currentRangeStart, 1);
      console.log('yearly -> ', this.currentRangeStart);

      this.createVisitorYearlyActivityChartCanvas(colors);
    }
  }
  public updateDateRange(direction: 'forward' | 'backward'): void {
    this.loadingService.loadingOn();
    const selectedType = this.visitorsChartType.value;
    if (this.visitorActivityChart) {
      this.visitorActivityChart.destroy();
    }
    let adjustmentValue = 0;
    if (selectedType === 1) adjustmentValue = 7; // Weekly
    else if (selectedType === 2) adjustmentValue = 1; // Monthly
    else if (selectedType === 3) adjustmentValue = 1; // Yearly

    if (direction === 'backward') adjustmentValue *= -1;

    if (selectedType === 1) {
      // Weekly
      this.currentRangeStart = addDays(this.currentRangeStart, adjustmentValue);
      this.currentRangeEnd = addDays(this.currentRangeEnd, adjustmentValue);
      console.log('weekly ->', this.currentRangeStart, this.currentRangeEnd);

      this.createVisitorWeekActivityChartCanvas(colors);
    } else if (selectedType === 2) {
      // Monthly
      this.currentRangeStart = addMonths(
        this.currentRangeStart,
        adjustmentValue
      );
      console.log('monthly ->', this.currentRangeStart);

      this.createVisitorMonthActivityChartCanvas(colors);
    } else if (selectedType === 3) {
      // Yearly
      this.currentRangeStart = addYears(
        this.currentRangeStart,
        adjustmentValue
      );
      console.log('early ->', this.currentRangeStart);

      this.createVisitorYearlyActivityChartCanvas(colors);
    }

    this.loadingService.loadingOff();
  }

  public getFormattedRange(): string {
    let date;
    const selectedType = this.visitorsChartType.value;

    if (selectedType === 1) {
      date = `${format(this.currentRangeStart, 'MMM dd yyyy')} - ${format(
        this.currentRangeEnd,
        'MMM dd yyyy'
      )}`;
    } else if (selectedType === 2) {
      date = `${format(this.currentRangeStart, 'MMM yyyy')}`;
    } else if (selectedType === 3) {
      date = `${format(this.currentRangeStart, 'yyyy')}`;
    }
    return date;
  }
}
