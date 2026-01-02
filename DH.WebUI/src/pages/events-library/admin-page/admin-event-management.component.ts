import { FrontEndLogService } from './../../../shared/services/frontend-log.service';
import { Component, OnDestroy, OnInit } from '@angular/core';
import { IMenuItem } from '../../../shared/models/menu-item.model';
import { MenuTabsService } from '../../../shared/services/menu-tabs.service';
import { NAV_ITEM_LABELS } from '../../../shared/models/nav-items-labels.const';
import { SearchService } from '../../../shared/services/search.service';
import { Router } from '@angular/router';
import { EventsService } from '../../../entities/events/api/events.service';
import { IEventListResult } from '../../../entities/events/models/event-list.model';
import { FULL_ROUTE } from '../../../shared/configs/route.config';
import { BehaviorSubject, Observable } from 'rxjs';
import { ControlsMenuComponent } from '../../../shared/components/menu/controls-menu.component';
import { DateHelper } from '../../../shared/helpers/date-helper';
import { EventConfirmDeleteDialog } from '../../../features/events-library/dialogs/event-confirm-delete/event-confirm-delete.component';
import { MatDialog } from '@angular/material/dialog';
import { ToastService } from '../../../shared/services/toast.service';
import { TranslateService } from '@ngx-translate/core';
import { AppToastMessage } from '../../../shared/components/toast/constants/app-toast-messages.constant';
import { ToastType } from '../../../shared/models/toast.model';
import { SupportLanguages } from '../../../entities/common/models/support-languages.enum';
import { LanguageService } from '../../../shared/services/language.service';

@Component({
    selector: 'app-admin-event-management',
    templateUrl: 'admin-event-management.component.html',
    styleUrl: 'admin-event-management.component.scss',
    standalone: false
})
export class AdminEventManagementComponent implements OnInit, OnDestroy {
  public headerMenuItems: BehaviorSubject<IMenuItem[]> = new BehaviorSubject<
    IMenuItem[]
  >([]);
  public itemMenuItems: BehaviorSubject<IMenuItem[]> = new BehaviorSubject<
    IMenuItem[]
  >([]);
  public visibleMenuId: number | null = null;
  public events: IEventListResult[] = [];

  public readonly DATE_TIME_FORMAT: string = DateHelper.DATE_TIME_FORMAT;

  constructor(
    private readonly menuTabsService: MenuTabsService,
    private readonly searchService: SearchService,
    private readonly eventService: EventsService,
    private readonly router: Router,
    private readonly dialog: MatDialog,
    private readonly toastService: ToastService,
    private readonly translateService: TranslateService,
    private readonly frontEndLogService: FrontEndLogService,
    private readonly languageService: LanguageService
  ) {
    this.menuTabsService.setActive(NAV_ITEM_LABELS.EVENTS);
    this.handleHeaderMenuItemClick = this.handleHeaderMenuItemClick.bind(this);
  }

  public showEventMenu(
    eventId: number,
    event: MouseEvent,
    menu: ControlsMenuComponent
  ): void {
    event.stopPropagation();
    this.visibleMenuId = this.visibleMenuId === eventId ? null : eventId;
    menu.toggleMenu();
  }

  public get currentLanguage(): SupportLanguages {
    return this.languageService.getCurrentLanguage();
  }

  public ngOnInit(): void {
    this.itemMenuItems.next([
      {
        key: 'update',
        label: this.translateService.instant('events.menu_items.update'),
      },
      {
        key: 'delete',
        label: this.translateService.instant('events.menu_items.delete'),
      },
      {
        key: 'send-notification',
        label: this.translateService.instant(
          'events.menu_items.send_notification'
        ),
      },
    ]);

    this.headerMenuItems.next([
      {
        key: 'add',
        label: this.translateService.instant('events.menu_items.add'),
      },
    ]);

    this.fetchEventList();
  }

  public ngOnDestroy(): void {
    this.menuTabsService.resetData();
    this.searchService.hideSearchForm();
  }

  public isEventExpired(eventDate: Date): boolean {
    const eventDateObj =
      eventDate instanceof Date ? eventDate : new Date(eventDate);
    const today = new Date();

    // Set both dates to the beginning of the day to ignore time differences
    const todayStartOfDay = new Date(
      today.getFullYear(),
      today.getMonth(),
      today.getDate(),
      today.getHours()
    );
    const eventStartOfDay = new Date(
      eventDateObj.getFullYear(),
      eventDateObj.getMonth(),
      eventDateObj.getDate(),
      eventDateObj.getHours()
    );

    return eventStartOfDay.getTime() < todayStartOfDay.getTime();
  }

  public handleEventMenuItemClick(key: string, event: MouseEvent): void {
    event.stopPropagation();
    if (key === 'update' && this.visibleMenuId) {
      this.router.navigateByUrl(
        FULL_ROUTE.EVENTS.ADMIN.UPDATE_BY_ID(this.visibleMenuId)
      );
    } else if (key === 'delete' && this.visibleMenuId) {
      this.openDeleteDialog(this.visibleMenuId);
    } else if (key === 'send-notification') {
      this.eventService.sendEventNotifications(this.visibleMenuId!).subscribe({
        next: () => {
          this.toastService.success({
            message: this.translateService.instant(
              'events.send_notifications_for_event'
            ),
            type: ToastType.Success,
          });
        },
        error: (error) => {
          this.frontEndLogService
            .sendError(
              `Sending notifications for event with id ${
                this.visibleMenuId
              } failed. Error ${JSON.stringify(error)}`,
              'admin-event-management.component.ts'
            )
            .subscribe();

          this.toastService.error({
            message: this.translateService.instant(
              AppToastMessage.SomethingWrong
            ),
            type: ToastType.Error,
          });
        },
      });
    }
    this.visibleMenuId = null;
  }
  private openDeleteDialog(id: number): void {
    const dialogRef = this.dialog.open(EventConfirmDeleteDialog, {
      data: { id: id },
    });

    dialogRef.afterClosed().subscribe((result) => {
      if (result) {
        this.fetchEventList();
      }
    });
  }

  public handleHeaderMenuItemClick(key: string): void {
    if (key === 'add') {
      this.router.navigateByUrl(FULL_ROUTE.EVENTS.ADMIN.ADD);
    }
  }

  public handleSearchExpression(searchExpression: string): void {
    this.fetchEventList(searchExpression);
  }

  public navigateToDetails(eventId: number): void {
    this.router.navigateByUrl(FULL_ROUTE.EVENTS.ADMIN.DETAILS_BY_ID(eventId));
  }

  private fetchEventList(searchExpression: string = ''): void {
    this.eventService.getListForStaff(searchExpression).subscribe({
      next: (eventList) => (this.events = eventList ?? []),
      error: () => {
        this.toastService.error({
          message: this.translateService.instant(
            AppToastMessage.SomethingWrong
          ),
          type: ToastType.Error,
        });
      },
    });
  }
}
