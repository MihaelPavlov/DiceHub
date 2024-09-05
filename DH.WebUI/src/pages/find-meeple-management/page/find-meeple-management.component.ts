import { Component, OnDestroy, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { MenuTabsService } from '../../../shared/services/menu-tabs.service';
import { NAV_ITEM_LABELS } from '../../../shared/models/nav-items-labels.const';
import { RoomsService } from '../../../entities/rooms/api/rooms.service';
import { IRoomListResult } from '../../../entities/rooms/models/room-list.model';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-find-meeple-manager',
  templateUrl: 'find-meeple-management.component.html',
  styleUrl: 'find-meeple-management.component.scss',
})
export class FindMeepleManagementComponent implements OnInit, OnDestroy {
  public roomList$!: Observable<IRoomListResult[] | null>;
  constructor(
    private readonly router: Router,
    private readonly menuTabsService: MenuTabsService,
    private readonly roomService: RoomsService
  ) {
    this.menuTabsService.setActive(NAV_ITEM_LABELS.MEEPLE);
  }

  public ngOnInit(): void {
    this.roomList$ = this.roomService.getList();
  }

  public ngOnDestroy(): void {
    this.menuTabsService.resetData();
  }

  public handleSeachExpression(searchExpression: string) {
    this.roomList$ = this.roomService.getList(searchExpression);
  }

  public navigateToCreateMeepleRoom(): void {
    this.router.navigateByUrl('meeples/create');
  }
}
