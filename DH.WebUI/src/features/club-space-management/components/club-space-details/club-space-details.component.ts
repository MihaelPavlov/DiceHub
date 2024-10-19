import { Observable } from 'rxjs';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Params, Router } from '@angular/router';
import { SpaceManagementService } from '../../../../entities/space-management/api/space-management.service';
import { IUserActiveSpaceTableResult } from '../../../../entities/space-management/models/user-active-space-table.model';
import { ISpaceTableParticipant } from '../../../../entities/space-management/models/table-participant.model';

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

  constructor(
    private readonly router: Router,
    private readonly activeRoute: ActivatedRoute,
    private readonly spaceManagementService: SpaceManagementService
  ) {}

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

  public onRemoveUserFromTable(userId: string): void {
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
