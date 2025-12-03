import { AfterViewInit, Component, ElementRef, ViewChild } from '@angular/core';
import { Chart, registerables } from 'chart.js';
import 'chartjs-adapter-date-fns';
import ChartDataLabels from 'chartjs-plugin-datalabels';

@Component({
    selector: 'chart',
    templateUrl: 'chart.component.html',
    styleUrl: 'chart.component.scss',
    standalone: false
})
export class Chart2Component implements AfterViewInit {
  @ViewChild('visitorMonthActivityChartCanvas')
  private visitorMonthActivityChartCanvas!: ElementRef<HTMLCanvasElement>;
  visitorMonthActivityChart: any;

  @ViewChild('visitorWeekActivityChartCanvas')
  private visitorWeekActivityChartCanvas!: ElementRef<HTMLCanvasElement>;
  visitorWeekActivityChart: any;

  @ViewChild('reservationChartCanvas')
  private reservationChartCanvas!: ElementRef<HTMLCanvasElement>;
  reservationChart: any;

  @ViewChild('rewardsChartCanvas')
  private rewardsChartCanvas!: ElementRef<HTMLCanvasElement>;
  rewardsChart: any;

  @ViewChild('eventAttendanceChartCanvas')
  private eventAttendanceChartCanvas!: ElementRef<HTMLCanvasElement>;
  eventAttendanceChart: any;

  @ViewChild('rewardsStatsChartCanvas')
  private rewardsStatsChartCanvas!: ElementRef<HTMLCanvasElement>;
  rewardsStatsChart: any;

  constructor() {
    Chart.register(ChartDataLabels, ...registerables);
  }
  ngAfterViewInit(): void {
    this.loadAllCharts();
  }

  ngOnInit(): void {
    console.log('Chart component initialized');

    // this.loadVisitorActivityData();
  }

  loadAllCharts() {
    const colors = {
      blue: {
        default: 'rgba(55, 156, 226, 1)', // #379ce2 in RGBA
        half: 'rgba(55, 156, 226, 0.5)', // Semi-transparent version of #379ce2
        quarter: 'rgba(55, 156, 226, 0.25)', // More transparent version
        zero: 'rgba(55, 156, 226, 0)', // Fully transparent
      },
      green: {
        default: 'rgba(82, 203, 164, 1)', // #52cba4 in RGBA
        half: 'rgba(82, 203, 164, 0.5)', // Semi-transparent version of #52cba4
        quarter: 'rgba(82, 203, 164, 0.25)', // More transparent version
        zero: 'rgba(82, 203, 164, 0)', // Fully transparent
      },
      green2: {
        default: 'rgba(74, 201, 95, 1)', // #4ac95f in RGBA
        half: 'rgba(74, 201, 95, 0.5)', // Semi-transparent version of #4ac95f
        quarter: 'rgba(74, 201, 95, 0.25)', // More transparent version
        zero: 'rgba(74, 201, 95, 0)', // Fully transparent
      },
      indigo: {
        default: 'rgba(55, 95, 131, 1)', // A complementary indigo color closer to #379ce2
        quarter: 'rgba(55, 95, 131, 0.25)', // Semi-transparent indigo
      },
      purple: {
        default: 'rgba(210, 112, 212, 1)', // #d270d4 in RGBA
        half: 'rgba(210, 112, 212, 0.5)', // Semi-transparent version of #d270d4
        quarter: 'rgba(210, 112, 212, 0.25)', // More transparent version
        zero: 'rgba(210, 112, 212, 0)', // Fully transparent
      },
      peach: {
        default: 'rgba(255, 152, 91, 1)', // #ff985b in RGBA
        half: 'rgba(255, 152, 91, 0.5)', // Semi-transparent version of #ff985b
        quarter: 'rgba(255, 152, 91, 0.25)', // More transparent version
        zero: 'rgba(255, 152, 91, 0)', // Fully transparent
      },
      purple2: {
        default: 'rgba(164, 96, 226, 1)', // #a460e2 in RGBA
        half: 'rgba(164, 96, 226, 0.5)', // Semi-transparent version of #a460e2
        quarter: 'rgba(164, 96, 226, 0.25)', // More transparent version
        zero: 'rgba(164, 96, 226, 0)', // Fully transparent
      },
      pinkPurple: {
        default: 'rgba(175, 73, 130, 1)', // #af4982 in RGBA
        half: 'rgba(175, 73, 130, 0.5)', // Semi-transparent version of #af4982
        quarter: 'rgba(175, 73, 130, 0.25)', // More transparent version
        zero: 'rgba(175, 73, 130, 0)', // Fully transparent
      },
      fadedPeach: {
        default: 'rgba(246, 172, 133, 1)', // #f6ac85 in RGBA
        half: 'rgba(246, 172, 133, 0.5)', // Semi-transparent version of #f6ac85
        quarter: 'rgba(246, 172, 133, 0.25)', // More transparent version
        zero: 'rgba(246, 172, 133, 0)', // Fully transparent
      },
    };

    this.createVisitorMonthActivityChartCanvas(colors);
    this.createVisitorWeekActivityChartCanvas(colors);
    this.createReservationChartCanvas(colors);
    this.createRewardsChartCanvas(colors);
    this.createDoughnutChart();

    this.createRewardsStatsChartCanvas(colors);
  }

  aggregateDataByDay(data: any[]): { labels: any[]; values: number[] } {
    const timeCounts: { [key: string]: number } = {}; // Use Set to count unique users

    // Count occurrences for each day, ensuring unique users
    data.forEach((log) => {
      const date = new Date(log.dateTime).toISOString().split('T')[0]; // Only keep date (no time)
      if (!timeCounts[date]) {
        timeCounts[date] = log.userCount;
      }
    });
    console.log(timeCounts);

    // Prepare chart data for daily visits
    const labels = Object.keys(timeCounts).map((date) => new Date(date));
    const values = Object.values(timeCounts).map((set) => set); // Count unique users
    console.log(labels, values);

    return { labels, values };
  }

  aggregateDataByMonth(data: any[]): { labels: any[]; values: number[] } {
    const timeCounts: { [key: string]: number } = {}; // Use Set to count unique users

    // Count occurrences for each month
    data.forEach((log) => {
      const month = new Date(log.dateTime).toISOString().slice(0, 7); // Get YYYY-MM for monthly grouping
      console.log(month);

      if (!timeCounts[month]) {
        timeCounts[month] = log.userCount;
      }
    });
    console.log(timeCounts);

    // Prepare chart data for monthly visits
    const labels = Object.keys(timeCounts).map((month) => month); // Use the 1st day of the month for labels
    const values = Object.values(timeCounts).map((set) => set); // Count unique users
    console.log(labels);
    console.log(values);

    return { labels, values };
  }

  createVisitorMonthActivityChartCanvas(colors): void {
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

    if (this.visitorMonthActivityChartCanvas) {
      const ctx1 =
        this.visitorMonthActivityChartCanvas.nativeElement.getContext('2d');
      // Monthly visitor activity
      if (ctx1) {
        ctx1.canvas.height = 100;

        let gradient = ctx1.createLinearGradient(0, 25, 0, 300);
        gradient.addColorStop(0, colors.green.half);
        gradient.addColorStop(0.35, colors.green.quarter);
        gradient.addColorStop(1, colors.green.zero);
        this.visitorMonthActivityChart = new Chart(ctx1, {
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
                pointBackgroundColor: '#52cba4',
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
                max: 35, // max should be bigger then the biggest userCount value
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

  createVisitorWeekActivityChartCanvas(colors): void {
    // Hardcoded visitor activity data
    const activityDataByWeek = [
      { userCount: 1, dateTime: '2024-12-20T10:30:00Z' },
      { userCount: 2, dateTime: '2024-12-21T10:45:00Z' },
      { userCount: 1, dateTime: '2024-12-22T11:00:00Z' },
      { userCount: 2, dateTime: '2024-12-23T11:30:00Z' },
      { userCount: 3, dateTime: '2024-12-24T11:15:00Z' },
      { userCount: 3, dateTime: '2024-12-25T12:00:00Z' },
      { userCount: 30, dateTime: '2024-12-26T12:15:00Z' },
      { userCount: 1, dateTime: '2024-12-27T11:45:00Z' },
      { userCount: 5, dateTime: '2024-12-28T12:00:00Z' }, // Visit in December (month)
      { userCount: 10, dateTime: '2024-12-30T12:30:00Z' }, // Visit in December (month)
    ];

    const dailyData = this.aggregateDataByDay(activityDataByWeek);

    // Visitors by week or 10 days
    const ctx2 =
      this.visitorWeekActivityChartCanvas.nativeElement.getContext('2d');
    if (ctx2) {
      ctx2.canvas.height = 100;

      let gradient = ctx2.createLinearGradient(0, 25, 0, 300);
      gradient.addColorStop(0, colors.blue.half);
      gradient.addColorStop(0.35, colors.blue.quarter);
      gradient.addColorStop(1, colors.blue.zero);
      this.visitorWeekActivityChart = new Chart(ctx2, {
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
              borderColor: colors.blue.quarter,
              pointBackgroundColor: '#379ce2',
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
              grid: {
                display: false,
              },
              type: 'time',
              time: {
                unit: 'day', // Use 'day' for daily data
              },
              // title: {
              //   display: true,
              //   text: 'Date/Time',
              // },
            },
            y: {
              max: 35, // max should be bigger then the biggest userCount value
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

  createReservationChartCanvas(colors): void {
    const ctx3 = this.reservationChartCanvas.nativeElement.getContext('2d');

    const reservationData = {
      table: { nonCancelled: 30, cancelled: 10 },
      game: { nonCancelled: 50, cancelled: 20 },
    };

    if (ctx3) {
      let gradientPurple = ctx3.createLinearGradient(0, 25, 0, 300);
      gradientPurple.addColorStop(0, colors.purple.half);
      gradientPurple.addColorStop(0.35, colors.purple.quarter);
      gradientPurple.addColorStop(1, colors.purple.zero);

      let gradientGreen = ctx3.createLinearGradient(0, 25, 0, 300);
      gradientGreen.addColorStop(0, colors.green2.default);
      gradientGreen.addColorStop(0.35, colors.green2.quarter);
      gradientGreen.addColorStop(1, colors.green2.zero);

      this.reservationChart = new Chart(ctx3, {
        type: 'bar',
        data: {
          labels: ['Table Reservations', 'Game Reservations'],
          datasets: [
            {
              label: 'Non-Cancelled',
              data: [
                reservationData.table.nonCancelled,
                reservationData.game.nonCancelled,
              ],
              backgroundColor: gradientPurple,
              borderColor: colors.purple.default,
              borderWidth: 1,
              borderRadius: 5,
            },
            {
              label: 'Cancelled',
              data: [
                reservationData.table.cancelled,
                reservationData.game.cancelled,
              ],
              backgroundColor: gradientGreen,
              borderColor: colors.green2.default,
              borderRadius: 5,
              borderWidth: 1,
            },
          ],
        },
        options: {
          responsive: true,
          maintainAspectRatio: false,
          plugins: {
            legend: {
              display: true,
            },
            tooltip: {
              callbacks: {
                label: (tooltipItem) => {
                  return `${tooltipItem.dataset.label}: ${tooltipItem.raw}`;
                },
              },
            },
            datalabels: {
              display: true,
              color: 'white',
            },
          },
          scales: {
            x: {
              grid: {
                color: '#31313b',
              },
              stacked: false, // Separate bars
            },
            y: {
              stacked: false, // Separate bars
              beginAtZero: true,
              grid: {
                color: '#31313b',
              },
            },
          },
        },
      });
    }
  }

  createRewardsChartCanvas(colors): void {
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

  createRewardsStatsChartCanvas(colors): void {
    const ctx = this.rewardsStatsChartCanvas.nativeElement.getContext('2d');
    const rewardsData = [
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
      { rewardName: 'Reward R', collectionCount: 300 },
    ];
    if (!ctx) return;

    let gradientPurple = ctx.createLinearGradient(0, 25, 0, 300);
    gradientPurple.addColorStop(0.35, colors.fadedPeach.half);
    gradientPurple.addColorStop(1, colors.fadedPeach.quarter);
    this.rewardsStatsChart = new Chart(ctx, {
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
  createDoughnutChart(): void {
    const ctx = this.eventAttendanceChartCanvas.nativeElement.getContext('2d');
    const attendanceData = [
      { userAttendedCount: 25, eventName: 'Yu gi OH' },
      { userAttendedCount: 40, eventName: 'Kids Night' },
      { userAttendedCount: 60, eventName: 'The Event of the year' },
      { userAttendedCount: 80, eventName: 'January Event' },
      { userAttendedCount: 25, eventName: 'Christmas Board Game Party ' },
      { userAttendedCount: 5, eventName: 'Event H' },
      { userAttendedCount: 60, eventName: 'Event J' },
      { userAttendedCount: 80, eventName: 'Event K' },
      { userAttendedCount: 25, eventName: 'Yu gi OH' },
      { userAttendedCount: 40, eventName: 'Kids Night' },
      { userAttendedCount: 60, eventName: 'The Event of the year' },
      { userAttendedCount: 80, eventName: 'January Event' },
      { userAttendedCount: 25, eventName: 'Christmas Board Game Party ' },
      { userAttendedCount: 5, eventName: 'Event H' },
      { userAttendedCount: 60, eventName: 'Event J' },
    ];
    if (!ctx) return;

    ctx.canvas.height = 100;

    // Generate colors for each event
    const eventColors = attendanceData.map((_, index) =>
      this.generateColor(index)
    );
    const plugin = {
      id: 'customCanvasBackgroundImage',
      beforeDraw: (chart) => {
        const ctx = chart.ctx;
        const { top, left, width, height } = chart.chartArea;
        const centerX = left + width / 2;
        const centerY = top + height / 2;

        // You can adjust this value or pass a dynamic value for the number
        const numberToDisplay = `${attendanceData.length} Events`; // Example number

        // Set text style
        ctx.save();
        ctx.font = 'bold 24px Arial'; // Customize font size and style
        ctx.fillStyle = 'white'; // Text color
        ctx.textAlign = 'center';
        ctx.textBaseline = 'middle';

        // Draw the number in the center
        ctx.fillText(numberToDisplay, centerX, centerY);

        ctx.restore();
      },
    };
    this.eventAttendanceChart = new Chart(ctx, {
      type: 'doughnut',
      data: {
        labels: attendanceData.map((data) => data.eventName), // Labels for segments
        datasets: [
          {
            label: 'Event Attendance',
            data: attendanceData.map((data) => data.userAttendedCount),
            backgroundColor: eventColors.map((color) =>
              this.generateGradient(ctx, color)
            ),
            hoverBackgroundColor: eventColors.map((color) => color.half),
            borderColor: eventColors.map((color) => color.default),
            borderRadius: 5,
            borderWidth: 1,
          },
        ],
      },
      plugins:[plugin],
      options: {
        responsive: true,
        maintainAspectRatio: false,
        plugins: {
          legend: {
            display: true,
            position: 'top',
          },
          datalabels: {
            display: true,
            color: 'white',
          },
          tooltip: {
            callbacks: {
              label: (tooltipItem) => {
                const value = tooltipItem.raw;
                const label = tooltipItem.label;
                return `${label}: ${value} attendees`;
              },
            },
          },
        },
      },
    });
  }

  generateGradient(
    ctx: CanvasRenderingContext2D,
    color: {
      default: string;
      half: string;
      quarter: string;
      zero: string;
    }
  ): CanvasGradient {
    var gradient = ctx.createLinearGradient(0, 25, 50, 300);
    gradient.addColorStop(0, color.default);
    gradient.addColorStop(0.35, color.half);
    gradient.addColorStop(1, color.quarter);
    return gradient;
  }

  generateColor(index: number): {
    default: string;
    half: string;
    quarter: string;
    zero: string;
  } {
    // Define base hues for variety
    // cool collers- >     const hues = [100, 120, 200, 360, 180, 100, 300, 180]; // Softer hues

    const hues = [100, 120, 200, 360, 180, 100, 300, 180]; // Softer hues
    const hue = hues[index % hues.length]; // Cycle through hues
    const saturation = 40; // Softer saturation (reduced intensity)
    const lightness = 60; // Increased lightness for pastel effect

    const toRgba = (alpha: number) =>
      `hsla(${hue}, ${saturation}%, ${lightness}%, ${alpha})`;

    return {
      default: toRgba(1), // Solid color
      half: toRgba(0.5), // 50% transparency
      quarter: toRgba(0.25), // 25% transparency
      zero: toRgba(0), // Fully transparent
    };
  }
}
