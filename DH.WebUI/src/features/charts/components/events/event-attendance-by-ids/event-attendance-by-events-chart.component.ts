import { ToastService } from './../../../../../shared/services/toast.service';
import { StatisticsService } from './../../../../../entities/statistics/api/statistics.service';
import { EventsService } from './../../../../../entities/events/api/events.service';
import { Component, OnDestroy, ViewChild, ElementRef } from '@angular/core';
import { Chart, registerables } from 'chart.js';
import { NAV_ITEM_LABELS } from '../../../../../shared/models/nav-items-labels.const';
import { MenuTabsService } from '../../../../../shared/services/menu-tabs.service';
import ChartDataLabels from 'chartjs-plugin-datalabels';
import { AppToastMessage } from '../../../../../shared/components/toast/constants/app-toast-messages.constant';
import { ToastType } from '../../../../../shared/models/toast.model';
import { combineLatest, debounceTime, Subject, tap } from 'rxjs';
import { IDropdown } from '../../../../../shared/models/dropdown.model';
import { TenantRouter } from '../../../../../shared/helpers/tenant-router';

@Component({
    selector: 'event-attendance-by-events-chart',
    templateUrl: 'event-attendance-by-events-chart.component.html',
    styleUrl: 'event-attendance-by-events-chart.component.scss',
    standalone: false
})
export class EventAttendanceByEventsChartComponent implements OnDestroy {
  @ViewChild('eventAttendanceChartCanvas')
  private eventAttendanceChartCanvas!: ElementRef<HTMLCanvasElement>;
  private eventAttendanceChart!: any;
  public selectedEventIds: IDropdown[] = [];
  public eventList: IDropdown[] = [];
  private eventSelectionSubject: Subject<IDropdown[]> = new Subject<
    IDropdown[]
  >();
  public dots: string = '';
  private intervalId: any;
  public isLoading: boolean = false;

  constructor(
    private readonly tenantRouter: TenantRouter,
    private readonly menuTabsService: MenuTabsService,
    private readonly eventsService: EventsService,
    private readonly statisticsService: StatisticsService,
    private readonly toastService: ToastService
  ) {
    this.menuTabsService.setActive(NAV_ITEM_LABELS.PROFILE);
    Chart.register(ChartDataLabels, ...registerables);

    this.eventsService.getAllEventsDropdownList().subscribe({
      next: (events) => {
        this.eventList = events;
      },
    });

    this.eventSelectionSubject
      .pipe(
        tap(() => {
          this.loading = true;
        }),
        debounceTime(3000)
      )
      .subscribe((selectedIds) => {
        this.createDoughnutChart(selectedIds);
      });
  }
  public onEventSelectionChange(): void {
    this.eventSelectionSubject.next(this.selectedEventIds);
  }
  public backNavigateBtn(): void {
    this.tenantRouter.navigateTenant('charts/events');
  }

  public ngOnDestroy(): void {
    this.menuTabsService.resetData();
  }
  public getName(item: any): void {
    return item.name;
  }

  public startDotAnimation(): void {
    this.intervalId = setInterval(() => {
      this.dots = this.dots.length < 3 ? this.dots + '.' : '';
    }, 500); // Change dots every 500ms
  }

  public stopDotAnimation(): void {
    clearInterval(this.intervalId);
    this.dots = '';
  }

  set loading(value: boolean) {
    if (value) {
      this.isLoading = true;
      this.startDotAnimation();
    } else {
      this.stopDotAnimation();
      this.isLoading = false;
    }
  }

  public createDoughnutChart(selectedIds: IDropdown[]): void {
    if (this.eventAttendanceChart) {
      this.eventAttendanceChart.destroy();
    }

    if (selectedIds.length !== 0) {
      this.loading = false;
      combineLatest([
        this.eventsService.getAllEventsDropdownList(),
        this.statisticsService.getEventAttendanceByIds(
          selectedIds.map((x) => x.id)
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
                  events.find((e) => e.id == x.eventId)?.name ||
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
