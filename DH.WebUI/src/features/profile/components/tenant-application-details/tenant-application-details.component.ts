import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { TenantApplicationsService } from '../../../../entities/common/api/tenant-applications.service';
import {
  ITenantApplication,
  TenantApplicationStatus,
} from '../../../../entities/common/models/tenant-application.model';
import { FULL_ROUTE } from '../../../../shared/configs/route.config';
import { TenantRouter } from '../../../../shared/helpers/tenant-router';
import { ToastService } from '../../../../shared/services/toast.service';
import { ToastType } from '../../../../shared/models/toast.model';
import { AppToastMessage } from '../../../../shared/components/toast/constants/app-toast-messages.constant';

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
  public isResendingSetupInvitation = false;

  constructor(
    private readonly route: ActivatedRoute,
    private readonly tenantApplicationsService: TenantApplicationsService,
    private readonly tenantRouter: TenantRouter,
    private readonly toastService: ToastService
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

  public isVerified(): boolean {
    return this.application?.status === TenantApplicationStatus.Verified;
  }

  public resendSetupInvitation(): void {
    if (!this.application || this.isResendingSetupInvitation) return;

    this.isResendingSetupInvitation = true;
    this.tenantApplicationsService
      .resendSetupInvitation(this.application.id)
      .subscribe({
        next: (isSent) => {
          if (isSent) {
            this.toastService.success({
              message: 'Setup invitation email resent.',
              type: ToastType.Success,
            });
          } else {
            this.toastService.error({
              message: AppToastMessage.SomethingWrong,
              type: ToastType.Error,
            });
          }
        },
        error: () => {
          this.toastService.error({
            message: AppToastMessage.SomethingWrong,
            type: ToastType.Error,
          });
        },
        complete: () => {
          this.isResendingSetupInvitation = false;
        },
      });
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
