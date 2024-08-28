import { Component, HostListener, OnDestroy, OnInit } from '@angular/core';
import { IMenuItem } from '../../../shared/models/menu-item.model';
import { MenuTabsService } from '../../../shared/services/menu-tabs.service';
import { NAV_ITEM_LABELS } from '../../../shared/models/nav-items-labels.const';
import { SearchService } from '../../../shared/services/search.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-admin-event-management',
  templateUrl: 'admin-event-management.component.html',
  styleUrl: 'admin-event-management.component.scss',
})
export class AdminEventManagementComponent implements OnInit, OnDestroy {
  public headerMenuItems: IMenuItem[] = [];
  public eventMenuItems: IMenuItem[] = [];
  public visibleMenuId: number | null = null;

  constructor(
    private readonly menuTabsService: MenuTabsService,
    private readonly searchService: SearchService,
    private readonly router: Router
  ) {
    this.menuTabsService.setActive(NAV_ITEM_LABELS.EVENTS);
    this.handleHeaderMenuItemClick = this.handleHeaderMenuItemClick.bind(this);
  }
  public showHeaderMenu(gameId: number, event: MouseEvent): void {
    event.stopPropagation();
    console.log('test', gameId);
  }

  public showEventMenu(gameId: number, event: MouseEvent): void {
    event.stopPropagation();
    console.log('test', gameId);

    this.visibleMenuId = this.visibleMenuId === gameId ? null : gameId;
  }

  public ngOnInit(): void {
    this.eventMenuItems = [
      { key: 'edit', label: 'Edit' },
      { key: 'delete', label: 'Delete' },
      { key: 'send-notification', label: 'Send Notificaion' },
    ];

    this.headerMenuItems = [{ key: 'add', label: 'Add Event' }];
  }

  public ngOnDestroy(): void {
    this.menuTabsService.resetData();
    this.searchService.hideSearchForm();
  }

  public handleEventMenuItemClick(key: string, event: MouseEvent): void {
    event.stopPropagation();
    if (key === 'edit') {
      this.router.navigateByUrl('/admin-events/add');
    } else if (key === 'delete') {
      this.router.navigateByUrl('/games/add-existing-game');
    } else if (key === 'send-notification') {
      this.router.navigateByUrl('/games/reservations');
    }
    this.visibleMenuId = null;
  }

  public handleHeaderMenuItemClick(key: string): void {
    if (key === 'add') {
      this.router.navigateByUrl('/admin-events/add');
    }
  }

  public handleSeachExpression(searchExpression: string) {}

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
