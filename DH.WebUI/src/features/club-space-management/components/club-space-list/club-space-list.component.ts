import { AuthService } from './../../../../entities/auth/auth.service';
import { Component, OnDestroy } from '@angular/core';
import { Observable } from 'rxjs';
import { SpaceManagementService } from '../../../../entities/space-management/api/space-management.service';
import { ISpaceTableList } from '../../../../entities/space-management/models/space-table-list.model';
import { MatDialog } from '@angular/material/dialog';
import { JoinTableConfirmDialog } from '../../dialogs/join-table-confirm-dialog/join-table-confirm-dialog.component';
import { Router } from '@angular/router';
import { SearchService } from '../../../../shared/services/search.service';
import { ImageEntityType } from '../../../../shared/pipe/entity-image.pipe';
import { MenuTabsService } from '../../../../shared/services/menu-tabs.service';
import { FULL_ROUTE } from '../../../../shared/configs/route.config';
import {
  ImagePreviewDialog,
  ImagePreviewData,
} from '../../../../shared/dialogs/image-preview/image-preview.dialog';
import { TranslateService } from '@ngx-translate/core';
import { TenantRouter } from '../../../../shared/helpers/tenant-router';

@Component({
    selector: 'app-club-space-list',
    templateUrl: 'club-space-list.component.html',
    styleUrl: 'club-space-list.component.scss',
    standalone: false
})
export class ClubSpaceListComponent implements OnDestroy {
  public spaceAvailableTableList$!: Observable<ISpaceTableList[] | null>;
  public readonly ImageEntityType = ImageEntityType;

  constructor(
    private readonly spaceManagementService: SpaceManagementService,
    private readonly authService: AuthService,
    private readonly dialog: MatDialog,
    private readonly tenantRouter: TenantRouter,
    private readonly searchService: SearchService,
    private readonly menuTabsService: MenuTabsService,
    private readonly translateService: TranslateService
  ) {}

  public ngOnDestroy(): void {
    this.menuTabsService.resetData();
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
      width: '18rem',
      data: { roomId, withPassword, error },
    });

    dialogRef.afterClosed().subscribe((result) => {
      if (result) {
        if (result.hasError) {
          this.onJoin(roomId, withPassword, result.errorMessage);
          return;
        }

        this.tenantRouter.navigateTenant(FULL_ROUTE.SPACE_MANAGEMENT.HOME);
      }
    });
  }

  public openImagePreview(imageUrl: string) {
    this.dialog.open<ImagePreviewDialog, ImagePreviewData>(ImagePreviewDialog, {
      data: {
        imageUrl,
        title: this.translateService.instant('image'),
      },
      width: '17rem',
    });
  }
}
