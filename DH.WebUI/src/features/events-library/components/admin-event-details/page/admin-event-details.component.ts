import { Component, OnDestroy, OnInit } from '@angular/core';
import { IEventByIdResult } from '../../../../../entities/events/models/event-by-id.mode';
import { Observable } from 'rxjs';
import { EventsService } from '../../../../../entities/events/api/events.service';
import { ActivatedRoute, Params, Router } from '@angular/router';
import { NAV_ITEM_LABELS } from '../../../../../shared/models/nav-items-labels.const';
import { MenuTabsService } from '../../../../../shared/services/menu-tabs.service';
import { FULL_ROUTE } from '../../../../../shared/configs/route.config';

@Component({
  selector: 'app-admin-event-details',
  templateUrl: 'admin-event-details.component.html',
  styleUrl: 'admin-event-details.component.scss',
})
export class AdminEventDetailsComponent implements OnInit, OnDestroy {
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
    this.menuTabsService.resetData();
  }

  public ngOnInit(): void {
    this.activeRoute.params.subscribe((params: Params) => {
      this.eventId = params['id'];
      console.log(this.eventId);

      this.fetchEvent();
    });
  }

  public navigateBackToEventList(): void {
    this.router.navigateByUrl(FULL_ROUTE.EVENTS.HOME);
  }

  public navigateToUpdate(id: number): void {
    this.router.navigateByUrl(FULL_ROUTE.EVENTS.ADMIN.UPDATE_BY_ID(id));
  }

  public getImage(event: IEventByIdResult): Observable<string>  {
    return this.eventService.getImage(event.isCustomImage, event.imageId);
  }

  private fetchEvent(): void {
    this.event$ = this.eventService.getById(this.eventId);
  }
}
