import { Component, OnDestroy, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { MenuTabsService } from '../../../shared/services/menu-tabs.service';
import { NAV_ITEM_LABELS } from '../../../shared/models/nav-items-labels.const';
import { EventsService } from '../../../entities/events/api/events.service';
import { IEventListResult } from '../../../entities/events/models/event-list.model';

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
    private readonly eventService: EventsService
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
    this.router.navigateByUrl(`events/${id}/details`);
  }

  public getImage(event: IEventListResult): string {
    if (event.isCustomImage) {
      return `https://localhost:7024/events/get-image/${event.imageId}`;
    }
    return `https://localhost:7024/games/get-image/${event.imageId}`;
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
