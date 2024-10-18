import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { SpaceManagementService } from '../../../entities/space-management/api/space-management.service';
import { combineLatest, throwError } from 'rxjs';
import { IUserActiveSpaceTableResult } from '../../../entities/space-management/models/user-active-space-table.model';
import { ISpaceActivityStats } from '../../../entities/space-management/models/space-activity-stats.model';

@Component({
  selector: 'app-club-space-management',
  templateUrl: 'club-space-management.component.html',
  styleUrl: 'club-space-management.component.scss',
})
export class ClubSpaceManagementComponent implements OnInit {
  public userActiveTableInfo!: IUserActiveSpaceTableResult;
  public spaceActivityStats!: ISpaceActivityStats;

  constructor(
    private readonly router: Router,
    private readonly spaceManagementService: SpaceManagementService
  ) {}

  public ngOnInit(): void {
    combineLatest([
      this.spaceManagementService.getUserActiveTable(),
      this.spaceManagementService.getSpaceActivityStats(),
    ]).subscribe({
      next: ([userActiveTableResult, spaceActivityStats]) => {
        if (userActiveTableResult)
          this.userActiveTableInfo = userActiveTableResult;

        if (spaceActivityStats) this.spaceActivityStats = spaceActivityStats;
      },
      error: (errors) => {
        throwError(() => errors);
      },
    });
  }

  public navigateToSpaceManagement(): void {
    this.router.navigateByUrl('/space/list');
  }

  public navigateToSpaceClubDetails(): void {
    this.router.navigateByUrl('/space/1/details');
  }
}
