import { Injectable } from '@angular/core';
import {
  ActivatedRouteSnapshot,
  CanActivate,
  Router,
  RouterStateSnapshot,
  UrlTree,
} from '@angular/router';
import { TenantContextService } from '../services/tenant-context.service';
import { TenantRouter } from '../helpers/tenant-router';
import { ROUTE } from '../configs/route.config';

@Injectable({ providedIn: 'root' })
export class RedirectIfTenantGuard implements CanActivate {
  constructor(
    private readonly router: Router,
    private readonly tenantRouter: TenantRouter,
    private readonly tenantContext: TenantContextService
  ) {}

  canActivate(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): boolean | UrlTree {
    if (state.url.includes(ROUTE.LOGIN)) {
      return true;
    }
    if (this.tenantContext.hasTenant()) {
      const tenantUrl = this.tenantRouter.buildTenantUrl(ROUTE.LOGIN);
      return this.router.parseUrl(tenantUrl);
    }

    return true;
  }
}
