import { Observable } from 'rxjs';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Params, Router } from '@angular/router';
import { SpaceManagementService } from '../../../../entities/space-management/api/space-management.service';
import { IUserActiveSpaceTableResult } from '../../../../entities/space-management/models/user-active-space-table.model';
import { ISpaceTableParticipant } from '../../../../entities/space-management/models/table-participant.model';
import { AuthService } from '../../../../entities/auth/auth.service';
import { UserRole } from '../../../../entities/auth/enums/roles.enum';
import { FULL_ROUTE } from '../../../../shared/configs/route.config';
import { IMenuItem } from '../../../../shared/models/menu-item.model';

@Component({
  selector: 'app-club-space-details',
  templateUrl: 'club-space-details.component.html',
  styleUrl: 'club-space-details.component.scss',
})
export class ClubSpaceDetailsComponent implements OnInit {
  public userActiveTable$!: Observable<IUserActiveSpaceTableResult>;
  public spaceTableParticipantList$!: Observable<
    ISpaceTableParticipant[] | null
  >;
  public tableId!: number;
  public menuItems: IMenuItem[] = [];

  constructor(
    private readonly router: Router,
    private readonly activeRoute: ActivatedRoute,
    private readonly spaceManagementService: SpaceManagementService,
    private readonly authService: AuthService
  ) {
    this.menuItems = [
      { key: 'settings', label: 'Settings' },
      { key: 'system-rewards', label: 'System Rewards' },
      { key: 'custom-challenges', label: 'Custom Challenges' },
    ];
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
    if (key === 'add') {
      this.router.navigateByUrl(FULL_ROUTE.EVENTS.ADMIN.ADD);
    }
  }

  public ngOnInit(): void {
    this.userActiveTable$ = this.spaceManagementService.getUserActiveTable();
    this.activeRoute.params.subscribe((params: Params) => {
      this.tableId = params['id'];
      this.spaceTableParticipantList$ =
        this.spaceManagementService.getSpaceTableParticipantList(this.tableId);
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
