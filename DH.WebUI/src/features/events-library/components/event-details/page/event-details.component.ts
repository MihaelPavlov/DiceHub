import { Component } from '@angular/core';
import { IEventByIdResult } from '../../../../../entities/events/models/event-by-id.mode';
import { Observable } from 'rxjs';
import { EventsService } from '../../../../../entities/events/api/events.service';
import { ActivatedRoute, Params, Router } from '@angular/router';
import { NAV_ITEM_LABELS } from '../../../../../shared/models/nav-items-labels.const';
import { MenuTabsService } from '../../../../../shared/services/menu-tabs.service';

@Component({
  selector: 'app-event-details',
  templateUrl: 'event-details.component.html',
  styleUrl: 'event-details.component.scss',
})
export class EventDetailsComponent {
  public event$!: Observable<IEventByIdResult>;
  private eventId!: number;
  
  constructor(
    private readonly eventService: EventsService,
    private readonly activeRoute: ActivatedRoute,
    private readonly menuTabsService: MenuTabsService,
    private readonly router: Router
  ) {
    this.menuTabsService.setActive(NAV_ITEM_LABELS.EVENTS);
  }
  
  public ngOnDestroy(): void {
    this.menuTabsService.resetData;
  }

  public ngOnInit(): void {
    this.activeRoute.params.subscribe((params: Params) => {
      this.eventId = params['id'];
      this.fetchEvent();
    });
  }

  public navigateBackToEventList(): void {
    this.router.navigateByUrl('/admin-events');
  }

  public fetchEvent(): void {
    this.event$ = this.eventService.getById(this.eventId);
  }
}
