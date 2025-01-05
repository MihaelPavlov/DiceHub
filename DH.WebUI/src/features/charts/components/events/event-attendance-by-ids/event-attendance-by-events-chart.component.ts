import { EventsService } from './../../../../../entities/events/api/events.service';
import {
  Component,
  AfterViewInit,
  OnDestroy,
  ViewChild,
  ElementRef,
} from '@angular/core';
import { Chart, registerables } from 'chart.js';
import { LoadingService } from '../../../../../shared/services/loading.service';
import { Router } from '@angular/router';
import { NAV_ITEM_LABELS } from '../../../../../shared/models/nav-items-labels.const';
import { MenuTabsService } from '../../../../../shared/services/menu-tabs.service';
import ChartDataLabels from 'chartjs-plugin-datalabels';
import { ControlsMenuComponent } from '../../../../../shared/components/menu/controls-menu.component';

interface IEventDropdownResult {
  id: number;
  name: string;
}
@Component({
  selector: 'event-attendance-by-events-chart',
  templateUrl: 'event-attendance-by-events-chart.component.html',
  styleUrl: 'event-attendance-by-events-chart.component.scss',
})
export class EventAttendanceByEventsChartComponent
  implements AfterViewInit, OnDestroy
{
  @ViewChild('eventAttendanceChartCanvas')
  private eventAttendanceChartCanvas!: ElementRef<HTMLCanvasElement>;
  private eventAttendanceChart: any;
  public selectedEventIds: IEventDropdownResult[] = [];
  public eventList: IEventDropdownResult[] = [];

  constructor(
    private readonly loadingService: LoadingService,
    private readonly router: Router,
    private readonly menuTabsService: MenuTabsService,
    private readonly eventsService: EventsService
  ) {
    this.menuTabsService.setActive(NAV_ITEM_LABELS.PROFILE);
    Chart.register(ChartDataLabels, ...registerables);

    this.eventsService.getAllEventsDropdownList().subscribe({
      next: (events) => {
        this.eventList = events;
      },
    });
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

  public createDoughnutChart(): void {}

  public ngOnDestroy(): void {
    this.menuTabsService.resetData();
  }
}
