import { StatisticsService } from './../../../../entities/statistics/api/statistics.service';
import { EventsService } from './../../../../entities/events/api/events.service';
import {
  AfterViewInit,
  Component,
  ElementRef,
  OnDestroy,
  ViewChild,
} from '@angular/core';
import { Router } from '@angular/router';
import { Chart, registerables } from 'chart.js';
import { NAV_ITEM_LABELS } from '../../../../shared/models/nav-items-labels.const';
import { MenuTabsService } from '../../../../shared/services/menu-tabs.service';
import ChartDataLabels from 'chartjs-plugin-datalabels';
import { FormControl } from '@angular/forms';
import { combineLatest } from 'rxjs';
import { ToastService } from '../../../../shared/services/toast.service';
import { ToastType } from '../../../../shared/models/toast.model';
import { AppToastMessage } from '../../../../shared/components/toast/constants/app-toast-messages.constant';

@Component({
    selector: 'event-attendance-chart',
    templateUrl: 'event-attendance-chart.component.html',
    styleUrl: 'event-attendance-chart.component.scss',
    standalone: false
})
export class EventAttendanceChartComponent implements AfterViewInit, OnDestroy {
  @ViewChild('eventAttendanceChartCanvas')
  private eventAttendanceChartCanvas!: ElementRef<HTMLCanvasElement>;
  private eventAttendanceChart!: Chart;
  private REQUIRED_MESSAGE_FROM_DATES: string =
    'Specifying from which date we start is required.';

  public fromDateControl = new FormControl(null);
  public toDateControl = new FormControl(null);
  public validationMessageFromDates: string | null =
    this.REQUIRED_MESSAGE_FROM_DATES;

  constructor(
    private readonly router: Router,
    private readonly menuTabsService: MenuTabsService,
    private readonly eventsService: EventsService,
    private readonly statisticsService: StatisticsService,
    private readonly toastService: ToastService
  ) {
    this.menuTabsService.setActive(NAV_ITEM_LABELS.PROFILE);

    Chart.register(ChartDataLabels, ...registerables);
    this.fromDateControl.valueChanges.subscribe(() => this.dateValidation());
    this.toDateControl.valueChanges.subscribe(() => this.dateValidation());
  }

  public dateValidation(): void {
    if (this.eventAttendanceChart) {
      this.eventAttendanceChart.destroy();
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

    this.createDoughnutChart();
  }

  public backNavigateBtn(): void {
    this.router.navigateByUrl('charts/events');
  }

  public ngAfterViewInit(): void {
    this.createDoughnutChart();
  }
  public ngOnDestroy(): void {
    this.menuTabsService.resetData();
  }

  public createDoughnutChart(): void {
    if (this.fromDateControl.value && this.toDateControl.value) {
      combineLatest([
        this.eventsService.getAllEventsDropdownList(),
        this.statisticsService.getEventAttendanceChartData(
          new Date(this.fromDateControl.value).toISOString(),
          new Date(this.toDateControl.value).toISOString()
        ),
      ]).subscribe({
        next: ([events, operation]) => {
          if (
            events &&
            operation &&
            operation.success &&
            operation.relatedObject
          ) {
            const attendanceData = operation.relatedObject.eventAttendances.map(
              (x) => ({
                userAttendedCount: x.userAttendedCount,
                eventName:
                  events.find((e) => e.id === x.eventId)?.name ||
                  'Event Deleted',
              })
            );
            if (this.eventAttendanceChartCanvas) {
              const ctx =
                this.eventAttendanceChartCanvas.nativeElement.getContext('2d');
              if (ctx) {
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
                    const count = attendanceData.length;

                    const numberToDisplay =
                      count > 0
                        ? `${count} ${count === 1 ? 'Event' : 'Events'}`
                        : 'No Events';

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
                        data: attendanceData.map(
                          (data) => data.userAttendedCount
                        ),
                        backgroundColor: eventColors.map((color) =>
                          this.generateGradient(ctx, color)
                        ),
                        hoverBackgroundColor: eventColors.map(
                          (color) => color.half
                        ),
                        borderColor: eventColors.map((color) => color.default),
                        borderRadius: 5,
                        borderWidth: 1,
                      },
                    ],
                  },
                  plugins: [plugin],
                  options: {
                    responsive: true,
                    maintainAspectRatio: false,
                    plugins: {
                      legend: {
                        display: true,
                        position: 'bottom',
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
    // const attendanceData = [
    //   { userAttendedCount: 25, eventName: 'Yu gi OH' },
    //   { userAttendedCount: 40, eventName: 'Kids Night' },
    //   { userAttendedCount: 60, eventName: 'The Event of the year' },
    //   { userAttendedCount: 80, eventName: 'January Event' },
    //   { userAttendedCount: 25, eventName: 'Christmas Board Game Party ' },
    //   { userAttendedCount: 5, eventName: 'Event H' },
    //   { userAttendedCount: 60, eventName: 'Event J' },
    //   { userAttendedCount: 80, eventName: 'Event K' },
    //   { userAttendedCount: 25, eventName: 'Yu gi OH' },
    //   { userAttendedCount: 40, eventName: 'Kids Night' },
    //   { userAttendedCount: 60, eventName: 'The Event of the year' },
    //   { userAttendedCount: 80, eventName: 'January Event' },
    //   { userAttendedCount: 25, eventName: 'Christmas Board Game Party ' },
    //   { userAttendedCount: 5, eventName: 'Event H' },
    //   { userAttendedCount: 60, eventName: 'Event J' },
    // ];
  }

  private generateGradient(
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

  private generateColor(index: number): {
    default: string;
    half: string;
    quarter: string;
    zero: string;
  } {
    // Define base hues for variety
    // cool colors- >     const hues = [100, 120, 200, 360, 180, 100, 300, 180]; // Softer hues

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
