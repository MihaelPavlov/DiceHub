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

  constructor(
    private tenantContextService: TenantContextService,
    private readonly tenantService: TenantService,
    private tenantRouter: TenantRouter
  ) {}

  public ngOnInit(): void {
    window.scrollTo({ top: 0, behavior: 'auto' });
    
    this.tenantService
      .getList()
      .subscribe((clubs) => (this.clubs = clubs ?? []));
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
      this.tenantContextService.tenantId = this.selectedClub.id;

      this.tenantRouter.navigateTenant(ROUTE.LOGIN);
    }
  }

  public navigateToLanding(): void {
    this.tenantRouter.navigateGlobal(['/']);
  }

  public getLogoFile(logoFileName: string | null): string {
    return `/shared/assets/images/tenant_logos/${logoFileName}`;
  }
}
