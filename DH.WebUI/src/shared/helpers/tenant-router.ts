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
    console.log('tenant router -> ', this.tenantService.tenantId);

    if (!this.tenantService.hasTenant()) {
      throw new Error('Cannot navigate to tenant route without tenant');
    }

    const fullPath = this.buildTenantUrl(path);
    console.log('tenant router -> ', fullPath);

    return this.router.navigateByUrl(fullPath);
  }

  public buildTenantUrl(path: string): string {
    const tenant = this.tenantService.tenantId;
    return [tenant, path].join('/');
  }

  public navigateGlobal(path: string | string[]) {
    const segments = Array.isArray(path) ? path : [path];
    return this.router.navigate(['/', ...segments]);
  }

   public navigateByUrl(path: string) {
    return this.router.navigateByUrl(path);
  }
}
