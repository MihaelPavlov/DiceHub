import { Injectable } from '@angular/core';
import {
  Router,
  ActivatedRouteSnapshot,
  RouterStateSnapshot,
  UrlTree,
} from '@angular/router';
import { Observable } from 'rxjs';
import { AuthService } from '../../../entities/auth/auth.service';
import { UserRole } from '../../../entities/auth/enums/roles.enum';
import { ROUTE } from '../../../shared/configs/route.config';
import { TenantRouter } from '../../../shared/helpers/tenant-router';

@Injectable({
  providedIn: 'root',
})
export class SettingsSuperAdminAccessGuard {
  constructor(
    private readonly authService: AuthService,
    private readonly router: Router,
    private readonly tenantRouter: TenantRouter
  ) {}

  public canActivate(
    _: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): Observable<boolean> | boolean | Promise<boolean> | UrlTree {
    if (this.authService.getUser?.role === UserRole.SuperAdmin) {
      return true;
    }
    const tenantUrl = this.tenantRouter.buildTenantUrl(ROUTE.PROFILE.CORE);

    return this.router.parseUrl(tenantUrl);
  }
}
