import { Component, OnDestroy, OnInit } from '@angular/core';
import { SearchService } from '../../../../shared/services/search.service';
import { MenuTabsService } from '../../../../shared/services/menu-tabs.service';
import { NAV_ITEM_LABELS } from '../../../../shared/models/nav-items-labels.const';
import { RoomsService } from '../../../../entities/rooms/api/rooms.service';
import { ActivatedRoute, Params } from '@angular/router';
import { IRoomMemberResult } from '../../../../entities/rooms/models/room-member.model';
import { combineLatest } from 'rxjs';
import { IRoomByIdResult } from '../../../../entities/rooms/models/room-by-id.model';
import { AuthService } from '../../../../entities/auth/auth.service';
import { Location } from '@angular/common';
import { MatDialog } from '@angular/material/dialog';
import { RoomMemberConfirmDeleteDialog } from '../../dialogs/room-member-confirm-delete/room-member-confirm-delete.component';
import { LanguageService } from '../../../../shared/services/language.service';
import { SupportLanguages } from '../../../../entities/common/models/support-languages.enum';
import { DateHelper } from '../../../../shared/helpers/date-helper';

@Component({
  selector: 'app-room-members',
  templateUrl: 'room-members.component.html',
  styleUrls: ['room-members.component.scss'],
})
export class RoomMembersComponent implements OnInit, OnDestroy {
  public roomId!: number;
  public room!: IRoomByIdResult;
  public members: IRoomMemberResult[] = [];
  public readonly DATE_FORMAT: string = DateHelper.DATE_FORMAT;

  constructor(
    private readonly menuTabsService: MenuTabsService,
    private readonly searchService: SearchService,
    private readonly roomService: RoomsService,
    private readonly activeRoute: ActivatedRoute,
    private readonly authService: AuthService,
    private readonly location: Location,
    private readonly dialog: MatDialog,
    private readonly languageService: LanguageService
  ) {
    this.menuTabsService.setActive(NAV_ITEM_LABELS.MEEPLE);
  }

  public ngOnInit(): void {
    this.activeRoute.params.subscribe((params: Params) => {
      this.roomId = +params['id'];
      this.fetchMembers();
    });
  }

  public get currentLanguage(): SupportLanguages {
    return this.languageService.getCurrentLanguage();
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
