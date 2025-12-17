import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, Router } from '@angular/router';
import { TenantService } from '../services/tenant-context.service';

@Injectable({ providedIn: 'root' })
export class TenantGuard implements CanActivate {
  constructor(
    private tenantService: TenantService,
    private readonly router: Router
  ) {}

  public canActivate(route: ActivatedRouteSnapshot): boolean {
    const tenant = route.paramMap.get('tenant');

    if (!tenant) {
      this.router.navigate(['/login']);
      return false;
    }

    this.tenantService.tenantId = tenant;
    return true;
  }
}
