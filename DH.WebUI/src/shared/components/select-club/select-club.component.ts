import { TenantRouter } from './../../helpers/tenant-router';
import { Component, OnInit } from '@angular/core';
import { TenantContextService } from '../../services/tenant-context.service';
import { ROUTE } from '../../configs/route.config';
import { TenantService } from '../../services/tenant.service';
import { ITenantListResult } from '../../../entities/common/models/tenant-list.model';

@Component({
  selector: 'app-select-club',
  templateUrl: 'select-club.component.html',
  styleUrls: ['select-club.component.scss'],
  standalone: false,
})
export class SelectClubComponent implements OnInit {
  clubs: ITenantListResult[] = [];
  selectedClub: ITenantListResult | null = null;
  isLoading = true;

  constructor(
    private tenantContextService: TenantContextService,
    private readonly tenantService: TenantService,
    private tenantRouter: TenantRouter
  ) {}

  public ngOnInit(): void {
    window.scrollTo({ top: 0, behavior: 'auto' });

    this.tenantService.getList().subscribe({
      next: (clubs) => {
        this.clubs = clubs ?? [];
        this.isLoading = false;
      },
      error: () => {
        this.isLoading = false;
      },
    });
  }

  public selectClub(club: ITenantListResult): void {
    if (this.selectedClub?.id === club.id) {
      this.selectedClub = null;
      return;
    }
    this.selectedClub = club;
  }

  public navigateToLogin(): void {
    if (this.selectedClub) {
      this.tenantContextService.setTenant(
        this.selectedClub.id,
        this.selectedClub.tenantName
      );

      this.tenantRouter.navigateTenant(ROUTE.LOGIN);
    }
  }

  public navigateToLanding(): void {
    this.tenantRouter.navigateGlobal(['/']);
  }

  public getLogoFile(logoFileName: string | null): string {
    if (!logoFileName) {
      return '/shared/assets/images/default-logos/dicehub_logo_1.png';
    }

    if (/^https?:\/\//i.test(logoFileName)) {
      return logoFileName;
    }

    return `/shared/assets/images/tenant_logos/${logoFileName}`;
  }
}
