import { Component, OnInit, OnDestroy } from '@angular/core';
import { EventsService } from '../../../entities/events/api/events.service';
import { IEventListResult } from '../../../entities/events/models/event-list.model';
import { NAV_ITEM_LABELS } from '../../../shared/models/nav-items-labels.const';
import { MenuTabsService } from '../../../shared/services/menu-tabs.service';
import { FULL_ROUTE } from '../../../shared/configs/route.config';
import { combineLatest } from 'rxjs';
import { DateHelper } from '../../../shared/helpers/date-helper';
import { LanguageService } from '../../../shared/services/language.service';
import { SupportLanguages } from '../../../entities/common/models/support-languages.enum';
import { TenantRouter } from '../../../shared/helpers/tenant-router';

@Component({
  selector: 'app-events-library',
  templateUrl: 'events-library.component.html',
  styleUrl: 'events-library.component.scss',
  standalone: false,
})
export class EventsLibraryComponent implements OnInit, OnDestroy {
  public todayEvents: IEventListResult[] = [];
  public upcomingEvents: IEventListResult[] = [];
  public userEvents: IEventListResult[] = [];

  public readonly DATE_TIME_FORMAT: string = DateHelper.DATE_TIME_FORMAT;
  public readonly DATE_FORMAT: string = DateHelper.DATE_FORMAT;
  public readonly SupportLanguages = SupportLanguages;

  constructor(
    private readonly tenantRouter: TenantRouter,
    private readonly menuTabsService: MenuTabsService,
    private readonly eventService: EventsService,
    private readonly languageService: LanguageService
  ) {
    this.menuTabsService.setActive(NAV_ITEM_LABELS.EVENTS);
  }

  public ngOnInit(): void {
    this.fetchEventList();
  }

  public get currentLanguage(): SupportLanguages {
    return this.languageService.getCurrentLanguage();
  }

  public ngOnDestroy(): void {
    this.menuTabsService.resetData();
  }

  public navigateToDetails(id: number): void {
    this.tenantRouter.navigateTenant(FULL_ROUTE.EVENTS.DETAILS_BY_ID(id));
  }

  public isUserParticipatedIn(eventId): boolean {
    return this.userEvents.find((x) => x.id == eventId) ? true : false;
  }

  private fetchEventList() {
    combineLatest([
      this.eventService.getListForUser(),
      this.eventService.getUserEvents(),
    ]).subscribe({
      next: ([eventList, userEvents]) => {
        this.userEvents = userEvents ?? [];
        this.filterEvents(eventList);
      },
    });
  }

  private filterEvents(events: IEventListResult[]) {
    const today = new Date();
    today.setHours(0, 0, 0, 0);

    this.todayEvents = events.filter((event) =>
      this.isToday(event.startDate, today)
    );
    this.upcomingEvents = events.filter((event) =>
      this.isUpcoming(event.startDate, today)
    );
  }

  private isToday(eventDate: string | Date, today: Date): boolean {
    const eventDateObj = new Date(eventDate);
    return eventDateObj.toDateString() === today.toDateString();
  }

  private isUpcoming(eventDate: string | Date, today: Date): boolean {
    const eventDateObj =
      eventDate instanceof Date ? eventDate : new Date(eventDate);

    // Set both dates to the beginning of the day to ignore time differences
    const todayStartOfDay = new Date(
      today.getFullYear(),
      today.getMonth(),
      today.getDate()
    );
    const eventStartOfDay = new Date(
      eventDateObj.getFullYear(),
      eventDateObj.getMonth(),
      eventDateObj.getDate()
    );

    return eventStartOfDay.getTime() > todayStartOfDay.getTime();
  }
}
