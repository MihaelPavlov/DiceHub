import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { TenantContextService } from '../services/tenant-context.service';

@Injectable({ providedIn: 'root' })
export class TenantRouter {
  constructor(
    private router: Router,
    private tenantService: TenantContextService
  ) {}

  public navigateTenant(path: string) {
    if (!this.tenantService.hasTenant()) {
      throw new Error('Cannot navigate to tenant route without tenant');
    }

    const tenant = this.tenantService.tenantId;
    const fullPath = [tenant, path].join('/');

    return this.router.navigateByUrl(fullPath);
  }

  public navigateGlobal(path: string | string[]) {
    const segments = Array.isArray(path) ? path : [path];
    return this.router.navigate(['/', ...segments]);
  }
}
