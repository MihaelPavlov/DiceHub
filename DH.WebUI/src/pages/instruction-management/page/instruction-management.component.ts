import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { NavigationService } from '../../../shared/services/navigation-service';
import { AuthService } from '../../../entities/auth/auth.service';
import { ROUTE } from '../../../shared/configs/route.config';
import { TranslateService } from '@ngx-translate/core';
import { TenantRouter } from '../../../shared/helpers/tenant-router';

@Component({
  selector: 'app-instruction-management',
  templateUrl: 'instruction-management.component.html',
  styleUrl: 'instruction-management.component.scss',
  standalone: false,
})
export class InstructionManagementComponent {
  public defaultLinks: any = [];

  constructor(
    private readonly router: Router,
    private readonly tenantRouter: TenantRouter,
    private readonly navigationService: NavigationService,
    private readonly authService: AuthService,
    private readonly ts: TranslateService
  ) {
    this.defaultLinks = [
      {
        name: this.ts.instant('instruction.how_to_install.title'),
        path: '/instructions/how_to_install',
      },
      {
        name: this.ts.instant('instruction.notifications.title'),
        path: '/instructions/notifications',
      },
      {
        name: this.ts.instant('instruction.reservation.title'),
        path: '/instructions/reservation',
      },
      {
        name: this.ts.instant('instruction.events.title'),
        path: '/instructions/events',
      },
      {
        name: this.ts.instant('instruction.challenges.title'),
        path: '/instructions/challenges',
      },
      {
        name: this.ts.instant('instruction.meeples.title'),
        path: '/instructions/meeples',
      },
    ];
  }

  public get isUserAuthenticated(): boolean {
    return this.authService.getUser !== null;
  }

  public navigateTo(path: string): void {
    this.navigationService.setPreviousUrl(this.router.url);
    this.router.navigateByUrl(path);
  }

  public backNavigateBtn() {
    if (this.isUserAuthenticated) {
      this.tenantRouter.navigateTenant(ROUTE.PROFILE.CORE);
    } else {
      this.router.navigateByUrl(ROUTE.LANDING);
    }
  }
}
