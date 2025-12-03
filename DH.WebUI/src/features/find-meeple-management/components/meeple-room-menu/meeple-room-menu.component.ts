import {
  Component,
  EventEmitter,
  HostListener,
  Input,
  OnInit,
  Output,
} from '@angular/core';
import { IMenuItem } from '../../../../shared/models/menu-item.model';
import { Router } from '@angular/router';
import { MatDialog } from '@angular/material/dialog';
import { RoomConfirmDeleteDialog } from '../../dialogs/room-confirm-delete/room-confirm-delete.component';
import { RoomConfirmLeaveDialog } from '../../dialogs/room-confirm-leave/room-confirm-leave.component';
import { AuthService } from '../../../../entities/auth/auth.service';
import { IRoomByIdResult } from '../../../../entities/rooms/models/room-by-id.model';
import { BehaviorSubject } from 'rxjs';
import { ControlsMenuComponent } from '../../../../shared/components/menu/controls-menu.component';
import { NavigationService } from '../../../../shared/services/navigation-service';
import { FULL_ROUTE } from '../../../../shared/configs/route.config';
import { TranslateService } from '@ngx-translate/core';

@Component({
    selector: 'app-meeple-room-menu',
    templateUrl: 'meeple-room-menu.component.html',
    styleUrl: 'meeple-room-menu.component.scss',
    standalone: false
})
export class MeepleRoomMenuComponent implements OnInit {
  @Input() isCurrentUserParticipateInRoom!: boolean;
  @Input() room!: IRoomByIdResult;
  @Output() fetchData: EventEmitter<void> = new EventEmitter<void>();

  public menuItems: BehaviorSubject<IMenuItem[]> = new BehaviorSubject<
    IMenuItem[]
  >([]);
  public isMenuVisible: boolean = false;

  constructor(
    private readonly router: Router,
    private readonly dialog: MatDialog,
    private readonly authService: AuthService,
    private readonly navigationService: NavigationService,
    private readonly translateService: TranslateService
  ) {}

  public ngOnInit(): void {
    this.updateMenuItems();
  }

  public showMenu(event: MouseEvent, controlMenu: ControlsMenuComponent): void {
    event.stopPropagation();
    controlMenu.toggleMenu();
  }

  public updateMenuItems(): void {
    this.menuItems.next([
      {
        key: 'update',
        label: this.translateService.instant('meeple.room.menu.update'),
        isVisible:
          this.room &&
          this.room.createdBy === this.currentUserId() &&
          this.isCurrentUserParticipateInRoom,
      },
      {
        key: 'group-members',
        label: this.translateService.instant('meeple.room.menu.group_members'),
        isVisible: true,
      },
      {
        key: 'leave-room',
        label: this.translateService.instant('meeple.room.menu.leave_room'),
        isVisible:
          this.room &&
          this.room.createdBy !== this.currentUserId() &&
          this.isCurrentUserParticipateInRoom,
        isRedTextOn: true,
      },
      {
        key: 'delete-room',
        label: this.translateService.instant('meeple.room.menu.delete_room'),
        isVisible:
          this.room &&
          this.room.createdBy === this.currentUserId() &&
          this.isCurrentUserParticipateInRoom,
        isRedTextOn: true,
      },
    ]);
  }

  public currentUserId(): string {
    return this.authService.getUser?.id || '';
  }

  public onMenuOption(key: string, event: MouseEvent): void {
    if (key === 'group-members') {
      this.router.navigateByUrl(
        FULL_ROUTE.MEEPLE_ROOM.CHAT_MEMBERS(this.room.id)
      );
    } else if (key === 'update') {
      this.router.navigateByUrl(FULL_ROUTE.MEEPLE_ROOM.UPDATE(this.room.id));
    } else if (key === 'leave-room') {
      this.openLeaveRoomDialog();
    } else if (key === 'delete-room') {
      this.openDeleteRoomDialog();
    }

    this.isMenuVisible = !this.isMenuVisible;
  }

  public openDeleteRoomDialog(): void {
    const dialogRef = this.dialog.open(RoomConfirmDeleteDialog, {
      data: { id: this.room.id },
    });

    dialogRef.afterClosed().subscribe((result) => {
      if (result) {
        this.router.navigateByUrl(FULL_ROUTE.MEEPLE_ROOM.FIND);
      }
    });
  }

  public openLeaveRoomDialog(): void {
    const dialogRef = this.dialog.open(RoomConfirmLeaveDialog, {
      data: { id: this.room.id },
    });

    dialogRef.afterClosed().subscribe((result) => {
      if (result) {
        this.fetchData.emit();
      }
    });
  }

  public navigateToGameDetails(): void {
    this.navigationService.setPreviousUrl(
      FULL_ROUTE.MEEPLE_ROOM.DETAILS_BY_ID(this.room.id)
    );
    this.router.navigateByUrl(FULL_ROUTE.GAMES.DETAILS(this.room.gameId));
  }

  @HostListener('document:click', ['$event'])
  private onClickOutside(event: Event): void {
    const targetElement = event.target as HTMLElement;

    // Check if the clicked element is within the menu or the button that toggles the menu
    if (
      this.isMenuVisible === true &&
      !targetElement.closest('.menu_container')
    ) {
      this.isMenuVisible = false;
    }
  }
}
