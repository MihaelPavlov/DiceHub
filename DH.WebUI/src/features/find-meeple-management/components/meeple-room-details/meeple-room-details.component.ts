import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { RoomsService } from '../../../../entities/rooms/api/rooms.service';
import { ActivatedRoute, Params, Router } from '@angular/router';
import { MenuTabsService } from '../../../../shared/services/menu-tabs.service';
import { NAV_ITEM_LABELS } from '../../../../shared/models/nav-items-labels.const';
import { IRoomByIdResult } from '../../../../entities/rooms/models/room-by-id.model';
import { combineLatest } from 'rxjs';
import { AuthService } from '../../../../entities/auth/auth.service';
import { MeepleRoomMenuComponent } from '../meeple-room-menu/meeple-room-menu.component';
import { ImageEntityType } from '../../../../shared/pipe/entity-image.pipe';
import { FULL_ROUTE } from '../../../../shared/configs/route.config';

@Component({
  selector: 'app-meeple-room-details',
  templateUrl: 'meeple-room-details.component.html',
  styleUrl: 'meeple-room-details.component.scss',
})
export class MeepleRoomDetailsComponent implements OnInit, OnDestroy {
  @ViewChild(MeepleRoomMenuComponent) menu!: MeepleRoomMenuComponent;
  public room!: IRoomByIdResult;
  public isCurrentUserParticipateInRoom: boolean = false;
  public roomId!: number;
  public errorMessage: string | null = null;
  public readonly ImageEntityType = ImageEntityType;

  constructor(
    private readonly roomService: RoomsService,
    private readonly activeRoute: ActivatedRoute,
    private readonly authService: AuthService,
    private readonly menuTabsService: MenuTabsService,
    private readonly router: Router
  ) {
    this.menuTabsService.setActive(NAV_ITEM_LABELS.MEEPLE);
  }

  public ngOnDestroy(): void {
    this.menuTabsService.resetData();
  }

  public ngOnInit(): void {
    this.activeRoute.params.subscribe((params: Params) => {
      this.roomId = params['id'];
      this.fetchData();
    });
  }

  public backNavigateBtn(): void {
    this.router.navigateByUrl(FULL_ROUTE.MEEPLE_ROOM.FIND);
  }

  public navigateToGameDetails(id: number): void {
    this.router.navigateByUrl(FULL_ROUTE.GAMES.DETAILS(id));
  }

  public navigateToChat(id: number): void {
    this.router.navigateByUrl(FULL_ROUTE.MEEPLE_ROOM.CHAT_ROOM_BY_ID(id));
  }

  public onJoinRoom(): void {
    this.roomService.join(this.roomId).subscribe({
      next: () => this.fetchData(),
      error: (error) => {
        if (error.error.detail) this.errorMessage = error.error.detail;
      },
    });
  }

  public onLeaveCompleted(): void {
    this.fetchData();
  }

  public currentUserId(): string {
    return this.authService.getUser?.id || '';
  }

  private fetchData(): void {
    combineLatest([
      this.roomService.getById(this.roomId),
      this.roomService.checkUserParticipateInRoom(this.roomId),
    ]).subscribe({
      next: ([room, isParticipate]) => {
        this.room = room;

        this.isCurrentUserParticipateInRoom =
          room.createdBy === this.authService.getUser?.id || isParticipate;

        if (this.menu) {
          this.menu.room = this.room;
          this.menu.isCurrentUserParticipateInRoom =
            this.isCurrentUserParticipateInRoom;
          this.menu.updateMenuItems();
        }
      },
    });
  }
}
