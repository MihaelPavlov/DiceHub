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
import { LoadingService } from '../../../../shared/services/loading.service';
import { MenuTabsService } from '../../../../shared/services/menu-tabs.service';
import ChartDataLabels from 'chartjs-plugin-datalabels';
import { FormControl } from '@angular/forms';
import { ControlsMenuComponent } from '../../../../shared/components/menu/controls-menu.component';

@Component({
  selector: 'event-attendance-chart',
  templateUrl: 'event-attendance-chart.component.html',
  styleUrl: 'event-attendance-chart.component.scss',
})
export class EventAttendanceChartComponent implements AfterViewInit, OnDestroy {
  @ViewChild('eventAttendanceChartCanvas')
  private eventAttendanceChartCanvas!: ElementRef<HTMLCanvasElement>;
  private eventAttendanceChart: any;
  public fromDateControl = new FormControl(null);
  public toDateControl = new FormControl(null);

  constructor(
    private readonly loadingService: LoadingService,
    private readonly router: Router,
    private readonly menuTabsService: MenuTabsService
  ) {
    this.menuTabsService.setActive(NAV_ITEM_LABELS.PROFILE);

    Chart.register(ChartDataLabels, ...registerables);
    this.toDateControl.valueChanges.subscribe(() => this.dateValidation());
  }

  public dateValidation(): void {
    // if toDateControl is changes, check if fromDateControl have value
    // if not
    //    show message the specifying from which date we start is important
    // If yes
    //    validate if the fromDateControl.value is smaller then toDateControl.value
  }

  public backNavigateBtn(): void {
    this.router.navigateByUrl('profile');
  }

  public showMenu(event: MouseEvent, controlMenu: ControlsMenuComponent): void {
    event.stopPropagation();
    controlMenu.toggleMenu();
  }

  public ngAfterViewInit(): void {
    this.createDoughnutChart();
  }
  public ngOnDestroy(): void {
    this.menuTabsService.resetData();
  }

  public createDoughnutChart(): void {
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
