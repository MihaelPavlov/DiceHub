import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../../../entities/auth/auth.service';
import { UserRole } from '../../../../entities/auth/enums/roles.enum';
import { TenantContextService } from '../../../../shared/services/tenant-context.service';
import { FULL_ROUTE, ROUTE } from '../../../../shared/configs/route.config';

@Component({
  selector: 'app-profile-settings-nav',
  templateUrl: 'profile-settings-nav.component.html',
  styleUrl: 'profile-settings-nav.component.scss',
  standalone: false,
})
export class ProfileSettingsNavComponent {
  constructor(
    private readonly authService: AuthService,
    private readonly tenantContextService: TenantContextService,
    private readonly router: Router
  ) {}

  public readonly link = {
    profile: this.getTenantLink(ROUTE.PROFILE.CORE),
    userSettings: this.getTenantLink(FULL_ROUTE.PROFILE.USER_SETTINGS),
    settings: this.getTenantLink(FULL_ROUTE.PROFILE.SETTINGS),
    clubInfo: this.getTenantLink(FULL_ROUTE.PROFILE.CLUB_INFO),
    employees: this.getTenantLink(FULL_ROUTE.PROFILE.EMPLOYEES),
    ownerDetails: this.getTenantLink(FULL_ROUTE.PROFILE.OWNER_DETAILS),
    jobs: this.getTenantLink(FULL_ROUTE.PROFILE.JOBS),
    applicants: this.getTenantLink(FULL_ROUTE.PROFILE.APPLICANTS),
    tenants: this.getTenantLink(`${ROUTE.PROFILE.CORE}/tenants`),
  };

  public get isUser(): boolean {
    return (
      this.authService.getUser?.role === UserRole.Staff ||
      this.authService.getUser?.role === UserRole.User
    );
  }

  public get isOwnerOrSuperAdmin(): boolean {
    return (
      this.authService.getUser?.role === UserRole.Owner ||
      this.authService.getUser?.role === UserRole.SuperAdmin
    );
  }

  public get isSuperAdmin(): boolean {
    return this.authService.getUser?.role === UserRole.SuperAdmin;
  }

  private getTenantLink(path: string): string {
    return `/${this.tenantContextService.tenantId}/${path}`;
  }
}
