import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { TenantApplicationsService } from '../../../../entities/common/api/tenant-applications.service';
import {
  ITenantApplication,
  TenantApplicationStatus,
} from '../../../../entities/common/models/tenant-application.model';
import { FULL_ROUTE } from '../../../../shared/configs/route.config';
import { TenantRouter } from '../../../../shared/helpers/tenant-router';

@Component({
  selector: 'app-tenant-application-details',
  templateUrl: 'tenant-application-details.component.html',
  styleUrl: 'tenant-application-details.component.scss',
  standalone: false,
})
export class TenantApplicationDetailsComponent implements OnInit {
  public application?: ITenantApplication;
  public readonly status = TenantApplicationStatus;
  public isLoading = true;
  public isSaving = false;

  constructor(
    private readonly route: ActivatedRoute,
    private readonly tenantApplicationsService: TenantApplicationsService,
    private readonly tenantRouter: TenantRouter
  ) {}

  public ngOnInit(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));

    this.tenantApplicationsService.getById(id).subscribe({
      next: (application) => {
        this.application = application;
        this.isLoading = false;
      },
      error: () => {
        this.isLoading = false;
      },
    });
  }

  public verify(): void {
    if (!this.application || !confirm('Verify this tenant application?')) return;

    this.isSaving = true;
    this.tenantApplicationsService
      .verify(this.application.id, {})
      .subscribe(() => this.navigateBack());
  }

  public reject(): void {
    if (!this.application || !confirm('Reject this tenant application?')) return;

    this.isSaving = true;
    this.tenantApplicationsService
      .reject(this.application.id, {})
      .subscribe(() => this.navigateBack());
  }

  public navigateBack(): void {
    if (window.location.pathname.startsWith('/admin')) {
      this.tenantRouter.navigateGlobal(['admin', 'applicants']);
      return;
    }

    this.tenantRouter.navigateTenant(FULL_ROUTE.PROFILE.APPLICANTS);
  }

  public isPending(): boolean {
    return (
      this.application?.status === TenantApplicationStatus.PendingVerification
    );
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
