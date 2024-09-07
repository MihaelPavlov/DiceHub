import { Component, EventEmitter, HostListener, Input, OnInit, Output } from '@angular/core';
import { IMenuItem } from '../../../../shared/models/menu-item.model';
import { Router } from '@angular/router';
import { MatDialog } from '@angular/material/dialog';
import { RoomConfirmDeleteDialog } from '../../dialogs/room-confirm-delete/room-confirm-delete.component';
import { RoomConfirmLeaveDialog } from '../../dialogs/room-confirm-leave/room-confirm-leave.component';
import { AuthService } from '../../../../entities/auth/auth.service';
import { IRoomByIdResult } from '../../../../entities/rooms/models/room-by-id.model';

@Component({
  selector: 'app-meeple-room-menu',
  templateUrl: 'meeple-room-menu.component.html',
  styleUrl: 'meeple-room-menu.component.scss',
})
export class MeepleRoomMenuComponent implements OnInit {
  @Input() isCurrentUserParticipateInRoom!: boolean;
  @Input() room!: IRoomByIdResult;
  @Output() fetchData: EventEmitter<void> = new EventEmitter<void>();

  public menuItems: IMenuItem[] = [];
  public isMenuVisible: boolean = false;

  constructor(
    private readonly router: Router,
    private readonly dialog: MatDialog,
    private readonly authService: AuthService
  ) {}

  public ngOnInit(): void {
    this.updateMenuItems();
  }

  public showMenu(): void {
    this.isMenuVisible = !this.isMenuVisible;
  }

  public updateMenuItems(): void {
    this.menuItems = [
      { key: 'group-members', label: 'Group Members', isVisible: true },
      {
        key: 'leave-room',
        label: 'Leave Room',
        isVisible:
          this.room &&
          this.room.createdBy !== this.currentUserId() &&
          this.isCurrentUserParticipateInRoom,
      },
      {
        key: 'delete-room',
        label: 'Delete Room',
        isVisible:
          this.room &&
          this.room.createdBy === this.currentUserId() &&
          this.isCurrentUserParticipateInRoom,
      },
    ];
  }

  public currentUserId(): string {
    return this.authService.getUser?.id || '';
  }

  public handleMenuItemClick(key: string): void {
    if (key === 'group-members') {
      this.router.navigateByUrl(`/meeples/${this.room.id}/members`);
    } else if (key === 'leave-room') {
      this.openLeaveRoomDialog();
    } else if (key === 'delete-room') {
      this.openDeleteRoomDialog();
    }

    this.isMenuVisible = !this.isMenuVisible;
  }

  public openDeleteRoomDialog(): void {
    const dialogRef = this.dialog.open(RoomConfirmDeleteDialog, {
      width: '17rem',
      position: { bottom: '80%', left: '2%' },
      data: { id: this.room.id },
    });

    dialogRef.afterClosed().subscribe((result) => {
      if (result) {
        this.router.navigateByUrl(`/meeples/find`);
      }
    });
  }

  public openLeaveRoomDialog(): void {
    const dialogRef = this.dialog.open(RoomConfirmLeaveDialog, {
      width: '17rem',
      position: { bottom: '80%', left: '2%' },
      data: { id: this.room.id },
    });

    dialogRef.afterClosed().subscribe((result) => {
      if (result) {
        this.fetchData.emit();
      }
    });
  }

  public navigateToGameDetails(): void {
    this.router.navigateByUrl(`games/${this.room.gameId}/details`);
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
