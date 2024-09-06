import { Component, OnDestroy, OnInit } from '@angular/core';
import { RoomsService } from '../../../../entities/rooms/api/rooms.service';
import { ActivatedRoute, Params, Router } from '@angular/router';
import { MenuTabsService } from '../../../../shared/services/menu-tabs.service';
import { NAV_ITEM_LABELS } from '../../../../shared/models/nav-items-labels.const';
import { IRoomByIdResult } from '../../../../entities/rooms/models/room-by-id.model';
import { combineLatest } from 'rxjs';
import { AuthService } from '../../../../entities/auth/auth.service';

@Component({
  selector: 'app-meeple-room-details',
  templateUrl: 'meeple-room-details.component.html',
  styleUrl: 'meeple-room-details.component.scss',
})
export class MeepleRoomDetailsComponent implements OnInit, OnDestroy {
  public room!: IRoomByIdResult;
  public isCurrentUserParticipateInRoom: boolean = false;
  private roomId!: number;

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
    this.router.navigateByUrl('meeples/find');
  }

  public navigateToGameDetails(id: number): void {
    this.router.navigateByUrl(`games/${id}/details`);
  }

  public navigateToChat(id: number): void {
    this.router.navigateByUrl(`meeples/${id}/chat`);
  }

  public onDeleteRoom(): void {}

  public onLeaveRoom(): void {
    // this.roomService.leaveRoom(this.roomId).subscribe(() => {
    //   this.router.navigateByUrl('meeples/find');
    // });
  }

  public onJoinRoom(): void {
    this.roomService.join(this.roomId).subscribe((_) => this.fetchData());
  }

  public currentUserId(): string {
    return this.authService.getUser?.id || '';
  }

  private fetchData(): void {
    combineLatest([
      this.roomService.getById(this.roomId),
      this.roomService.checkUserParticipateInRoom(this.roomId),
    ]).subscribe(([room, isParticipate]) => {
      this.room = room;
      console.log('isUserParticipate', isParticipate);

      this.isCurrentUserParticipateInRoom =
        room.createdBy === this.authService.getUser?.id || isParticipate;
      console.log(this.isCurrentUserParticipateInRoom);
    });
  }
}
