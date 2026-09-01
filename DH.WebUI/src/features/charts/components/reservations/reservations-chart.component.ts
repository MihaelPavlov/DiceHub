import { ToastService } from './../../../../shared/services/toast.service';
import ChartDataLabels from 'chartjs-plugin-datalabels';
import {
  Component,
  AfterViewInit,
  ViewChild,
  ElementRef,
  OnDestroy,
} from '@angular/core';
import { Chart, registerables } from 'chart.js';
import { MenuTabsService } from '../../../../shared/services/menu-tabs.service';
import { NAV_ITEM_LABELS } from '../../../../shared/models/nav-items-labels.const';
import { FormControl } from '@angular/forms';
import { colors } from '../../consts/colors.const';
import { StatisticsService } from '../../../../entities/statistics/api/statistics.service';
import { GetReservationChartData } from '../../../../entities/statistics/models/reservation-chart.model';
import { OperationResult } from '../../../../shared/models/operation-result.model';
import { AppToastMessage } from '../../../../shared/components/toast/constants/app-toast-messages.constant';
import { ToastType } from '../../../../shared/models/toast.model';
import { ROUTE } from '../../../../shared/configs/route.config';
import { TenantRouter } from '../../../../shared/helpers/tenant-router';

@Component({
    selector: 'reservations-chart',
    templateUrl: 'reservations-chart.component.html',
    styleUrl: 'reservations-chart.component.scss',
    standalone: false
})
export class ReservationsChartComponent implements AfterViewInit, OnDestroy {
  @ViewChild('reservationChartCanvas')
  private reservationChartCanvas!: ElementRef<HTMLCanvasElement>;
  private reservationChart!: Chart;
  private REQUIRED_MESSAGE_FROM_DATES: string =
    'Specifying from which date we start is required.';

  public fromDateControl = new FormControl<string | null>(null);
  public toDateControl = new FormControl<string | null>(null);
  public validationMessageFromDates: string | null =
    this.REQUIRED_MESSAGE_FROM_DATES;
  public currentQuickRangeMonths: number | null = null;
  public quickRanges = [
    { label: 'Last Month', months: 1 },
    { label: 'Last 3 Months', months: 3 },
    { label: 'Last 6 Months', months: 6 },
  ];

  constructor(
    private readonly tenantRouter: TenantRouter,
    private readonly statisticsService: StatisticsService,
    private readonly menuTabsService: MenuTabsService,
    private readonly toastService: ToastService
  ) {
    this.menuTabsService.setActive(NAV_ITEM_LABELS.PROFILE);
    this.fromDateControl.valueChanges.subscribe(() => this.dateValidation());
    this.toDateControl.valueChanges.subscribe(() => this.dateValidation());

    Chart.register(ChartDataLabels, ...registerables);
  }

  public dateValidation(): void {
    this.currentQuickRangeMonths = null;
    if (this.reservationChart) {
      this.reservationChart.destroy();
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
    this.createReservationChartCanvas(colors);
  }

  public applyQuickRange(months: number): void {
    this.currentQuickRangeMonths = months;

    const toDate = new Date();
    const fromDate = new Date(toDate);
    fromDate.setMonth(fromDate.getMonth() - months);

    if (this.reservationChart) {
      this.reservationChart.destroy();
    }

    this.fromDateControl.setValue(this.formatDateInput(fromDate), {
      emitEvent: false,
    });
    this.toDateControl.setValue(this.formatDateInput(toDate), {
      emitEvent: false,
    });

    this.validationMessageFromDates = null;
    this.createReservationChartCanvas(colors);
  }

  public ngOnDestroy(): void {
    this.menuTabsService.resetData();
  }
  public ngAfterViewInit(): void {
    this.createReservationChartCanvas(colors);
  }

  public backNavigateBtn(): void {
    this.tenantRouter.navigateTenant(ROUTE.PROFILE.CORE);
  }

  private createReservationChartCanvas(colors): void {
    if (this.reservationChart) {
      this.reservationChart.destroy();
    }

    if (this.fromDateControl.value && this.toDateControl.value) {
      this.statisticsService
        .getReservationChartData(
          new Date(this.fromDateControl.value).toISOString(),
          new Date(this.toDateControl.value).toISOString()
        )
        .subscribe({
          next: (
            operation: OperationResult<GetReservationChartData> | null
          ) => {
            if (operation && operation.success && operation.relatedObject) {
              const reservationData = operation.relatedObject;

              if (this.reservationChartCanvas) {
                const ctx =
                  this.reservationChartCanvas.nativeElement.getContext('2d');

                if (ctx) {
                  let gradientPurple = ctx.createLinearGradient(0, 25, 0, 300);
                  gradientPurple.addColorStop(0, colors.purple.half);
                  gradientPurple.addColorStop(0.35, colors.purple.quarter);
                  gradientPurple.addColorStop(1, colors.purple.zero);

                  let gradientGreen = ctx.createLinearGradient(0, 25, 0, 300);
                  gradientGreen.addColorStop(0, colors.green2.default);
                  gradientGreen.addColorStop(0.35, colors.green2.quarter);
                  gradientGreen.addColorStop(1, colors.green2.zero);

                  this.reservationChart = new Chart(ctx, {
                    type: 'bar',
                    data: {
                      labels: ['Table Reservations', 'Game Reservations'],
                      datasets: [
                        {
                          label: 'Completed',
                          data: [
                            reservationData.tableReservationStats.completed,
                            reservationData.gameReservationStats.completed,
                          ],
                          backgroundColor: gradientPurple,
                          borderColor: colors.purple.default,
                          borderWidth: 1,
                          borderRadius: 5,
                        },
                        {
                          label: 'Cancelled',
                          data: [
                            reservationData.tableReservationStats.cancelled,
                            reservationData.gameReservationStats.cancelled,
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
  }

  private formatDateInput(date: Date): string {
    const year = date.getFullYear();
    const month = `${date.getMonth() + 1}`.padStart(2, '0');
    const day = `${date.getDate()}`.padStart(2, '0');

    return `${year}-${month}-${day}`;
  }
}
