import { Component, OnInit, OnDestroy } from '@angular/core';
import { SafeUrl } from '@angular/platform-browser';
import { Router } from '@angular/router';
import { EventsService } from '../../../entities/events/api/events.service';
import { IEventListResult } from '../../../entities/events/models/event-list.model';
import { NAV_ITEM_LABELS } from '../../../shared/models/nav-items-labels.const';
import { EventImagePipe } from '../../../shared/pipe/event-image.pipe';
import { GameImagePipe } from '../../../shared/pipe/game-image.pipe';
import { MenuTabsService } from '../../../shared/services/menu-tabs.service';
import { FULL_ROUTE } from '../../../shared/configs/route.config';

@Component({
  selector: 'app-events-library',
  templateUrl: 'events-library.component.html',
  styleUrl: 'events-library.component.scss',
})
export class EventsLibraryComponent implements OnInit, OnDestroy {
  public todayEvents: IEventListResult[] = [];
  public upcomingEvents: IEventListResult[] = [];

  constructor(
    private readonly router: Router,
    private readonly menuTabsService: MenuTabsService,
    private readonly eventService: EventsService,
    private readonly eventImagePipe: EventImagePipe,
    private readonly gameImagePipe: GameImagePipe
  ) {
    this.menuTabsService.setActive(NAV_ITEM_LABELS.EVENTS);
  }

  public ngOnInit(): void {
    this.fetchEventList();
  }

  public ngOnDestroy(): void {
    this.menuTabsService.resetData();
  }

  public navigateToDetails(id: number): void {
    this.router.navigateByUrl(FULL_ROUTE.EVENTS.DETAILS_BY_ID(id));
  }

  public getImage(event: IEventListResult): SafeUrl | null {
    if (event.isCustomImage) {
      return this.eventImagePipe.transform(event.imageId);
    }
    return this.gameImagePipe.transform(event.imageId);
  }

  private fetchEventList() {
    this.eventService.getList().subscribe({
      next: (eventList) => {
        this.filterEvents(eventList ?? []);
      },
      error: (error) => {
        console.log(error);
      },
    });
  }

  private filterEvents(events: IEventListResult[]) {
    const today = new Date();
    today.setHours(0, 0, 0, 0); // Set to midnight to compare dates only

    this.todayEvents = events.filter((event) =>
      this.isToday(event.startDate, today)
    );
    this.upcomingEvents = events.filter((event) =>
      this.isUpcoming(event.startDate, today)
    );
  }

  private isToday(eventDate: string | Date, today: Date): boolean {
    const eventDateObj = new Date(eventDate);
    return eventDateObj.getTime() === today.getTime();
  }

  private isUpcoming(eventDate: string | Date, today: Date): boolean {
    const eventDateObj = new Date(eventDate);
    return eventDateObj.getTime() > today.getTime();
  }
}
