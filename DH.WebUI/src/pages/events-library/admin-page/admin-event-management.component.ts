import { Component, HostListener, OnDestroy, OnInit } from '@angular/core';
import { IMenuItem } from '../../../shared/models/menu-item.model';
import { MenuTabsService } from '../../../shared/services/menu-tabs.service';
import { NAV_ITEM_LABELS } from '../../../shared/models/nav-items-labels.const';
import { SearchService } from '../../../shared/services/search.service';
import { Router } from '@angular/router';
import { EventsService } from '../../../entities/events/api/events.service';
import { IEventListResult } from '../../../entities/events/models/event-list.model';
import { GameImagePipe } from '../../../shared/pipe/game-image.pipe';
import { EventImagePipe } from '../../../shared/pipe/event-image.pipe';
import { SafeUrl } from '@angular/platform-browser';
import { FULL_ROUTE } from '../../../shared/configs/route.config';

@Component({
  selector: 'app-admin-event-management',
  templateUrl: 'admin-event-management.component.html',
  styleUrl: 'admin-event-management.component.scss',
})
export class AdminEventManagementComponent implements OnInit, OnDestroy {
  public headerMenuItems: IMenuItem[] = [];
  public eventMenuItems: IMenuItem[] = [];
  public visibleMenuId: number | null = null;
  public events: IEventListResult[] = [];

  constructor(
    private readonly menuTabsService: MenuTabsService,
    private readonly searchService: SearchService,
    private readonly eventService: EventsService,
    private readonly router: Router,
    private readonly gameImagePipe: GameImagePipe,
    private readonly eventImagePipe: EventImagePipe
  ) {
    this.menuTabsService.setActive(NAV_ITEM_LABELS.EVENTS);
    this.handleHeaderMenuItemClick = this.handleHeaderMenuItemClick.bind(this);
  }
  public showHeaderMenu(gameId: number, event: MouseEvent): void {
    event.stopPropagation();
    console.log('test', gameId);
  }

  public showEventMenu(eventId: number, event: MouseEvent): void {
    event.stopPropagation();
    console.log('test', eventId);

    this.visibleMenuId = this.visibleMenuId === eventId ? null : eventId;
  }

  public ngOnInit(): void {
    this.eventMenuItems = [
      { key: 'update', label: 'Update' },
      { key: 'delete', label: 'Delete' },
      { key: 'send-notification', label: 'Send Notificaion' },
    ];

    this.headerMenuItems = [{ key: 'add', label: 'Add Event' }];

    this.fetchEventList();
  }

  public ngOnDestroy(): void {
    this.menuTabsService.resetData();
    this.searchService.hideSearchForm();
  }

  public handleEventMenuItemClick(key: string, event: MouseEvent): void {
    event.stopPropagation();
    if (key === 'update' && this.visibleMenuId) {
      this.router.navigateByUrl(
        FULL_ROUTE.EVENTS.ADMIN.UPDATE_BY_ID(this.visibleMenuId)
      );
    } else if (key === 'delete') {
      //TODO: Add event delete
      this.router.navigateByUrl('/games/add-existing-game');
    } else if (key === 'send-notification') {
      this.router.navigateByUrl('/games/reservations');
    }
    this.visibleMenuId = null;
  }

  public handleHeaderMenuItemClick(key: string): void {
    if (key === 'add') {
      this.router.navigateByUrl(FULL_ROUTE.EVENTS.ADMIN.ADD);
    }
  }

  public handleSeachExpression(searchExpression: string) {
    this.fetchEventList(searchExpression);
  }

  public calculateRemainingDays(startDate: Date): string {
    const currentDate = new Date();
    const startDateSubject = new Date(startDate.toString());
    const remainingDays = Math.ceil(
      (startDateSubject.getTime() - currentDate.getTime()) /
        (1000 * 60 * 60 * 24)
    );
    return `${Math.abs(remainingDays)}d`;
  }

  public getImage(event: IEventListResult): SafeUrl | null {
    if (event.isCustomImage) {
      return this.eventImagePipe.transform(event.imageId);
    }
    return this.gameImagePipe.transform(event.imageId);
  }

  public navigateToDetails(eventId: number): void {
    this.router.navigateByUrl(FULL_ROUTE.EVENTS.ADMIN.DETAILS_BY_ID(eventId));
  }

  private fetchEventList(searchExpression: string = '') {
    this.eventService.getList(searchExpression).subscribe({
      next: (gameList) => (this.events = gameList ?? []),
      error: (error) => {
        console.log(error);
      },
    });
  }

  @HostListener('window:scroll', [])
  private onWindowScroll(): void {
    if (this.visibleMenuId !== null) {
      this.visibleMenuId = null;
    }
  }

  @HostListener('document:click', ['$event'])
  private onClickOutside(event: Event): void {
    const targetElement = event.target as HTMLElement;

    // Check if the clicked element is within the menu or the button that toggles the menu
    if (
      this.visibleMenuId !== null &&
      !targetElement.closest('.wrapper_library__item')
    ) {
      this.visibleMenuId = null;
    }
  }
}
