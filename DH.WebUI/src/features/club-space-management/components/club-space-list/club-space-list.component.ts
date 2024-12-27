import { AuthService } from './../../../../entities/auth/auth.service';
import { Component, ElementRef, OnDestroy } from '@angular/core';
import { Observable } from 'rxjs';
import { SpaceManagementService } from '../../../../entities/space-management/api/space-management.service';
import { ISpaceTableList } from '../../../../entities/space-management/models/space-table-list.model';
import { MatDialog } from '@angular/material/dialog';
import { JoinTableConfirmDialog } from '../../dialogs/join-table-confirm-dialog/join-table-confirm-dialog.component';
import { Router } from '@angular/router';
import { SearchService } from '../../../../shared/services/search.service';

@Component({
  selector: 'app-club-space-list',
  templateUrl: 'club-space-list.component.html',
  styleUrl: 'club-space-list.component.scss',
})
export class ClubSpaceListComponent implements OnDestroy {
  public spaceAvailableTableList$!: Observable<ISpaceTableList[] | null>;

  constructor(
    private readonly spaceManagementService: SpaceManagementService,
    private readonly authService: AuthService,
    private readonly dialog: MatDialog,
    private readonly router: Router,
    private readonly searchService: SearchService
  ) {}
  
  public ngOnDestroy(): void {
    this.searchService.hideSearchForm();
  }

  public get getCurrentUserId(): string | undefined {
    return this.authService.getUser?.id;
  }

  public ngOnInit(): void {
    this.spaceAvailableTableList$ =
      this.spaceManagementService.getSpaceAvailableTableList();
  }

  public handleSearchExpression(searchExpression: string) {
    this.spaceAvailableTableList$ =
      this.spaceManagementService.getSpaceAvailableTableList(searchExpression);
  }

  public onJoin(
    roomId: number,
    withPassword: boolean,
    error: string = ''
  ): void {
    const dialogRef = this.dialog.open(JoinTableConfirmDialog, {
      width: '17rem',
      data: { roomId, withPassword, error },
    });

    dialogRef.afterClosed().subscribe((result) => {
      if (result) {
        console.log('closedResult->', result);
        if (result.hasError) {
          this.onJoin(roomId, withPassword, result.errorMessage);
          return;
        }

        this.router.navigateByUrl('space/home');
      }
    });
  }
}
