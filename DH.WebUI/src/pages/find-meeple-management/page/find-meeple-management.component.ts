import { Component, OnDestroy, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { MenuTabsService } from '../../../shared/services/menu-tabs.service';
import { NAV_ITEM_LABELS } from '../../../shared/models/nav-items-labels.const';
import { RoomsService } from '../../../entities/rooms/api/rooms.service';
import { IRoomListResult } from '../../../entities/rooms/models/room-list.model';
import { Observable } from 'rxjs';
import { SearchService } from '../../../shared/services/search.service';
import { ImageEntityType } from '../../../shared/pipe/entity-image.pipe';
import { FULL_ROUTE } from '../../../shared/configs/route.config';
import { NavigationService } from '../../../shared/services/navigation-service';
import { DateHelper } from '../../../shared/helpers/date-helper';

@Component({
  selector: 'app-find-meeple-manager',
  templateUrl: 'find-meeple-management.component.html',
  styleUrl: 'find-meeple-management.component.scss',
})
export class FindMeepleManagementComponent implements OnInit, OnDestroy {
  public roomList$!: Observable<IRoomListResult[] | null>;

  public readonly ImageEntityType = ImageEntityType;
  public readonly DATE_TIME_FORMAT: string = DateHelper.DATE_TIME_FORMAT;

  constructor(
    private readonly router: Router,
    private readonly menuTabsService: MenuTabsService,
    private readonly roomService: RoomsService,
    private readonly searchService: SearchService,
    private readonly navigationService: NavigationService
  ) {
    this.menuTabsService.setActive(NAV_ITEM_LABELS.MEEPLE);
  }

  public ngOnInit(): void {
    this.roomList$ = this.roomService.getList();
  }

  public ngOnDestroy(): void {
    this.menuTabsService.resetData();
    this.searchService.hideSearchForm();
  }

  public handleSearchExpression(searchExpression: string) {
    this.roomList$ = this.roomService.getList(searchExpression);
  }

  public navigateToDetails(id: number): void {
    this.navigationService.setPreviousUrl(
      FULL_ROUTE.MEEPLE_ROOM.DETAILS_BY_ID(id)
    );
    this.router.navigateByUrl(FULL_ROUTE.MEEPLE_ROOM.DETAILS_BY_ID(id));
  }

  public navigateToCreateMeepleRoom(): void {
    this.router.navigateByUrl(FULL_ROUTE.MEEPLE_ROOM.CREATE);
  }
}
