import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { ROUTE } from '../../../shared/configs/route.config';
import { TenantRouter } from '../../../shared/helpers/tenant-router';
import { TenantContextService } from '../../../shared/services/tenant-context.service';

@Component({
  selector: 'app-register-choice',
  templateUrl: 'register-choice.component.html',
  styleUrl: 'register-choice.component.scss',
  standalone: false,
})
export class RegisterChoiceComponent {
  constructor(
    private readonly router: Router,
    private readonly tenantRouter: TenantRouter,
    private readonly tenantContextService: TenantContextService
  ) {}

  public navigateToPlayerRegistration(): void {
    if (!this.tenantContextService.hasTenant()) {
      this.router.navigateByUrl(ROUTE.CHOOSE_CLUB);
      return;
    }

    this.tenantRouter.navigateTenant(`${ROUTE.REGISTER}/player`);
  }

  public navigateToVenueApplication(): void {
    this.router.navigateByUrl('/venue-application');
  }

  public navigateToLogin(): void {
    if (!this.tenantContextService.hasTenant()) {
      this.router.navigateByUrl(ROUTE.CHOOSE_CLUB);
      return;
    }

    this.tenantRouter.navigateTenant(ROUTE.LOGIN);
  }

  public navigateToLanding(): void {
    this.router.navigateByUrl(ROUTE.LANDING);
  }
}
