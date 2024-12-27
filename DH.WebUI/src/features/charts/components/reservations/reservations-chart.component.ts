import ChartDataLabels from 'chartjs-plugin-datalabels';
import {
  Component,
  AfterViewInit,
  ViewChild,
  ElementRef,
  OnDestroy,
} from '@angular/core';
import { Chart, registerables } from 'chart.js';
import { Router } from '@angular/router';
import { LoadingService } from '../../../../shared/services/loading.service';
import { MenuTabsService } from '../../../../shared/services/menu-tabs.service';
import { NAV_ITEM_LABELS } from '../../../../shared/models/nav-items-labels.const';
import { FormControl } from '@angular/forms';
import { colors } from '../../consts/colors.const';
import { ControlsMenuComponent } from '../../../../shared/components/menu/controls-menu.component';

@Component({
  selector: 'reservations-chart',
  templateUrl: 'reservations-chart.component.html',
  styleUrl: 'reservations-chart.component.scss',
})
export class ReservationsChartComponent implements AfterViewInit, OnDestroy {
  @ViewChild('reservationChartCanvas')
  private reservationChartCanvas!: ElementRef<HTMLCanvasElement>;
  private reservationChart: any;
  public fromDateControl = new FormControl(null);
  public toDateControl = new FormControl(null);

  constructor(
    private readonly loadingService: LoadingService,
    private readonly router: Router,
    private readonly menuTabsService: MenuTabsService
  ) {
    this.menuTabsService.setActive(NAV_ITEM_LABELS.PROFILE);
    this.toDateControl.valueChanges.subscribe(() => this.dateValidation());

    Chart.register(ChartDataLabels, ...registerables);
  }

  public dateValidation(): void {
    // if toDateControl is changes, check if fromDateControl have value
    // if not
    //    show message the specifying from which date we start is important
    // If yes
    //    validate if the fromDateControl.value is smaller then toDateControl.value
  }

  public ngOnDestroy(): void {
    this.menuTabsService.resetData();
  }
  public ngAfterViewInit(): void {
    this.createReservationChartCanvas(colors);
  }

  public backNavigateBtn(): void {
    this.router.navigateByUrl('profile');
  }

  public showMenu(event: MouseEvent, controlMenu: ControlsMenuComponent): void {
    event.stopPropagation();
    controlMenu.toggleMenu();
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
