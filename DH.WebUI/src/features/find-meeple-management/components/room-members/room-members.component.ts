import { Component, OnDestroy, OnInit } from '@angular/core';
import { SearchService } from '../../../../shared/services/search.service';
import { MenuTabsService } from '../../../../shared/services/menu-tabs.service';
import { NAV_ITEM_LABELS } from '../../../../shared/models/nav-items-labels.const';
import { RoomsService } from '../../../../entities/rooms/api/rooms.service';
import { ActivatedRoute, Params } from '@angular/router';
import { IRoomMemberResult } from '../../../../entities/rooms/models/room-member.model';
import { ToastService } from '../../../../shared/services/toast.service';
import { AppToastMessage } from '../../../../shared/components/toast/constants/app-toast-messages.constant';
import { ToastType } from '../../../../shared/models/toast.model';
import { combineLatest } from 'rxjs';
import { IRoomByIdResult } from '../../../../entities/rooms/models/room-by-id.model';
import { AuthService } from '../../../../entities/auth/auth.service';
import { Location } from '@angular/common';
import { MatDialog } from '@angular/material/dialog';
import { RoomMemberConfirmDeleteDialog } from '../../dialogs/room-member-confirm-delete/room-member-confirm-delete.component';

@Component({
  selector: 'app-room-members',
  templateUrl: 'room-members.component.html',
  styleUrls: ['room-members.component.scss'],
})
export class RoomMembersComponent implements OnInit, OnDestroy {
  public roomId!: number;
  public room!: IRoomByIdResult;
  public members: IRoomMemberResult[] = [];

  constructor(
    private readonly menuTabsService: MenuTabsService,
    private readonly searchService: SearchService,
    private readonly roomService: RoomsService,
    private readonly activeRoute: ActivatedRoute,
    private readonly toastService: ToastService,
    private readonly authService: AuthService,
    private readonly location: Location,
    private readonly dialog: MatDialog
  ) {
    this.menuTabsService.setActive(NAV_ITEM_LABELS.MEEPLE);
  }
  public ngOnInit(): void {
    this.activeRoute.params.subscribe((params: Params) => {
      this.roomId = +params['id'];
      this.fetchMembers();
    });
  }

  public ngOnDestroy(): void {
    this.menuTabsService.resetData();
    this.searchService.hideSearchForm();
  }

  public getCurrentUserId(): string {
    return this.authService.getUser ? this.authService.getUser.id : '';
  }

  public handleSearchExpression(searchExpression: string) {
    this.fetchMembers(searchExpression);
  }

  public onBack(): void {
    this.location.back();
  }

  public onDelete(userId: string): void {
    const dialogRef = this.dialog.open(RoomMemberConfirmDeleteDialog, {
      data: { roomId: this.roomId, userId },
    });

    dialogRef.afterClosed().subscribe((result) => {
      if (result) {
        this.fetchMembers();
      }
    });
  }

  private fetchMembers(searchExpression: string = ''): void {
    combineLatest([
      this.roomService.getById(this.roomId),
      this.roomService.getMembers(this.roomId, searchExpression),
    ]).subscribe({
      next: ([room, members]) => {
        if (members) {
          this.members = members;
        }
        if (room) {
          this.room = room;
        }
      },
    });
  }
}
