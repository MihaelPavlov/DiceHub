import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, Router } from '@angular/router';
import { TenantContextService } from '../services/tenant-context.service';
import { ROUTE } from '../configs/route.config';

@Injectable({ providedIn: 'root' })
export class TenantGuard implements CanActivate {
  constructor(
    private tenantContextService: TenantContextService,
    private readonly router: Router
  ) {}

  public canActivate(route: ActivatedRouteSnapshot): boolean {
    const tenant = route.paramMap.get('tenant');

    if (!tenant) {
      this.router.navigateByUrl(ROUTE.CHOOSE_CLUB);
      return false;
    }

    this.tenantContextService.tenantId = tenant;
    return true;
  }
}
