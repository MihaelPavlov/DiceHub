import { Component, OnInit } from '@angular/core';
import { TenantApplicationsService } from '../../../../entities/common/api/tenant-applications.service';
import {
  ITenantApplication,
  TenantApplicationStatus,
} from '../../../../entities/common/models/tenant-application.model';
import { FULL_ROUTE } from '../../../../shared/configs/route.config';
import { TenantRouter } from '../../../../shared/helpers/tenant-router';

@Component({
  selector: 'app-tenant-applications',
  templateUrl: 'tenant-applications.component.html',
  styleUrl: 'tenant-applications.component.scss',
  standalone: false,
})
export class TenantApplicationsComponent implements OnInit {
  public applications: ITenantApplication[] = [];
  public readonly status = TenantApplicationStatus;
  public isLoading = true;

  constructor(
    private readonly tenantApplicationsService: TenantApplicationsService,
    private readonly tenantRouter: TenantRouter
  ) {}

  public ngOnInit(): void {
    this.tenantApplicationsService.getList().subscribe({
      next: (applications) => {
        this.applications = applications;
        this.isLoading = false;
      },
      error: () => {
        this.isLoading = false;
      },
    });
  }

  public open(application: ITenantApplication): void {
    if (this.isSystemAdminRoute()) {
      this.tenantRouter.navigateGlobal([
        'admin',
        'applicants',
        application.id.toString(),
      ]);
      return;
    }

    this.tenantRouter.navigateTenant(FULL_ROUTE.PROFILE.APPLICANT_DETAILS_BY_ID(application.id));
  }

  private isSystemAdminRoute(): boolean {
    return window.location.pathname.startsWith('/admin');
  }

  public getStatusLabel(status: TenantApplicationStatus): string {
    switch (status) {
      case TenantApplicationStatus.Verified:
        return 'Verified';
      case TenantApplicationStatus.Rejected:
        return 'Rejected';
      default:
        return 'Pending verification';
    }
  }
}
