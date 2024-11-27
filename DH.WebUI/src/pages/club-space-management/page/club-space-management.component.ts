import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { SpaceManagementService } from '../../../entities/space-management/api/space-management.service';
import { combineLatest, throwError } from 'rxjs';
import { IUserActiveSpaceTableResult } from '../../../entities/space-management/models/user-active-space-table.model';
import { ISpaceActivityStats } from '../../../entities/space-management/models/space-activity-stats.model';
import {
  ActiveBookedTableModel,
  getKeyFriendlyNames,
} from '../../../entities/space-management/models/active-booked-table.model';

@Component({
  selector: 'app-club-space-management',
  templateUrl: 'club-space-management.component.html',
  styleUrl: 'club-space-management.component.scss',
})
export class ClubSpaceManagementComponent implements OnInit {
  public userActiveTableInfo!: IUserActiveSpaceTableResult;
  public spaceActivityStats!: ISpaceActivityStats;
  public activeBookedTableModel: ActiveBookedTableModel | null = null;
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

    this.spaceManagementService.getActiveBookedTable().subscribe({
      next: (result) => {
        this.activeBookedTableModel = result;
      },
      error: (error) => {
        console.log(error);
      },
    });
  }

  public navigateSpaceTableList(): void {
    this.router.navigateByUrl('/space/list');
  }

  public navigateSpaceTableBooking(): void {
    this.router.navigateByUrl('/space/booking');
  }

  public navigateToCreateTable(): void {
    this.router.navigateByUrl('/qr-code-scanner');
  }

  public navigateToSpaceClubDetails(id: number | null | undefined): void {
    if (id) this.router.navigateByUrl(`/space/${id}/details`);
  }

  public getKeyValuePair(): { key: string; value: any }[] {
    if (this.activeBookedTableModel) {
      const keyFriendlyNames = getKeyFriendlyNames();
      const keyTransformations = this.getKeyTransformations();
  
      return Object.entries(this.activeBookedTableModel)
        .filter(([key]) => keyFriendlyNames[key] && keyTransformations[key])
        .map(([key, value]) => {
          const friendlyName = keyFriendlyNames[key]; 
          const transformedValue = keyTransformations[key]?.(value) ?? value;
  
          return { key: friendlyName, value: transformedValue };
        });
    }
    return [];
  }

  private getKeyTransformations(): Record<string, (value: any) => any> {
    return {
      username: (value) => value,
      numberOfGuests: (value) => value,
      reservationDate: (value) =>
        new Date(value).toISOString().replace('T', ' ').substring(0, 16),
      createdDate: (value) => new Date(value).toISOString().replace('T', ' ').substring(0, 10),
      isConfirmed: (value) => value,
    };
  }
}
