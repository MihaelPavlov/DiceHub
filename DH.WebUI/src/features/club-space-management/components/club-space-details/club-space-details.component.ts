import { Observable, BehaviorSubject } from 'rxjs';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Params, Router } from '@angular/router';
import { SpaceManagementService } from '../../../../entities/space-management/api/space-management.service';
import { IUserActiveSpaceTableResult } from '../../../../entities/space-management/models/user-active-space-table.model';
import { ISpaceTableParticipant } from '../../../../entities/space-management/models/table-participant.model';
import { AuthService } from '../../../../entities/auth/auth.service';
import { UserRole } from '../../../../entities/auth/enums/roles.enum';
import { FULL_ROUTE } from '../../../../shared/configs/route.config';
import { IMenuItem } from '../../../../shared/models/menu-item.model';
import { ISpaceTableById } from '../../../../entities/space-management/models/get-space-table-by-id.model';
import { NavigationService } from '../../../../shared/services/navigation-service';

@Component({
  selector: 'app-club-space-details',
  templateUrl: 'club-space-details.component.html',
  styleUrl: 'club-space-details.component.scss',
})
export class ClubSpaceDetailsComponent implements OnInit {
  public userActiveTable!: IUserActiveSpaceTableResult;
  public spaceTableParticipantList$!: Observable<
    ISpaceTableParticipant[] | null
  >;
  public tableId!: number;
  public menuItems: BehaviorSubject<IMenuItem[]> = new BehaviorSubject<
    IMenuItem[]
  >([]);

  public detailsSpaceTable!: ISpaceTableById;
  constructor(
    private readonly router: Router,
    private readonly activeRoute: ActivatedRoute,
    private readonly spaceManagementService: SpaceManagementService,
    private readonly authService: AuthService,
    private readonly navigationService: NavigationService
  ) {}
  public get isUserCreatorOfTable() {
    return this.detailsSpaceTable.createdBy === this.authService.getUser?.id;
  }

  public get isUserAdmin(): boolean {
    if (this.authService.getUser) {
      return this.authService.getUser.role !== UserRole.User ? true : false;
    }
    return false;
  }

  public handleMenuItemClick(key: string) {
    this.menuItemClickFunction(key);
  }

  public menuItemClickFunction(key: string): void {
    if (key === 'update') {
      this.navigationService.setPreviousUrl(this.router.url);
      this.router.navigateByUrl(
        FULL_ROUTE.SPACE_MANAGEMENT.UPDATE_BY_ID(this.tableId)
      );
    } else if (key === 'add-virtual-user') {
      this.addVirtualParticipant();
    } else if (key === 'close-room') {
      this.onClose();
    } else if (key === 'leave-room') {
      this.onLeave();
    }
  }

  public ngOnInit(): void {
    this.spaceManagementService.getUserActiveTable().subscribe({
      next: (result) => {
        this.userActiveTable = result;
      },
    });
    this.activeRoute.params.subscribe((params: Params) => {
      this.tableId = params['id'];
      this.spaceTableParticipantList$ =
        this.spaceManagementService.getSpaceTableParticipantList(this.tableId);

      this.spaceManagementService.getTableById(this.tableId).subscribe({
        next: (result) => {
          this.detailsSpaceTable = result;
          this.menuItems.next([
            {
              key: 'update',
              label: 'Update',
              isVisible: this.isUserCreatorOfTable,
            },
            {
              key: 'add-virtual-user',
              label: 'Add Virtual User',
              isVisible: this.isUserAdmin,
            },
            {
              key: 'close-room',
              label: 'Close Room',
              isVisible: this.isUserCreatorOfTable,
            },
            {
              key: 'leave-room',
              label: 'Leave Room',
              isVisible: this.userActiveTable
                ? this.userActiveTable.isPlayerParticipateInTable
                : false,
            },
          ]);
        },
      });
    });
  }

  public backToSpaceHome(): void {
    this.router.navigateByUrl('space/home');
  }

  public handleSearchExpression(searchExpression: string) {}

  public onLeave(): void {
    this.spaceManagementService.leaveTable(this.tableId).subscribe({
      next: () => {
        this.router.navigateByUrl('space/home');
      },
    });
  }

  public onClose(): void {
    this.spaceManagementService.closeTable(this.tableId).subscribe({
      next: () => {
        this.router.navigateByUrl('space/home');
      },
    });
  }

  public addVirtualParticipant(): void {
    this.spaceManagementService.addVirtualParticipant(this.tableId).subscribe({
      next: () => {
        this.spaceTableParticipantList$ =
          this.spaceManagementService.getSpaceTableParticipantList(
            this.tableId
          );
      },
    });
  }

  public onRemoveUserFromTable(
    userId: string,
    isVirtualParticipant: boolean,
    participantId: number
  ): void {
    if (isVirtualParticipant) {
      this.spaceManagementService
        .removeVirtualParticipant(this.tableId, participantId)
        .subscribe({
          next: () => {
            this.spaceTableParticipantList$ =
              this.spaceManagementService.getSpaceTableParticipantList(
                this.tableId
              );
          },
        });
    } else {
      this.spaceManagementService
        .removeUserFromTable(this.tableId, userId)
        .subscribe({
          next: () => {
            this.spaceTableParticipantList$ =
              this.spaceManagementService.getSpaceTableParticipantList(
                this.tableId
              );
          },
        });
    }
  }
}
