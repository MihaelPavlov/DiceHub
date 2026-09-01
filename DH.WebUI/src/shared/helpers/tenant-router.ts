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
    // `path` may come in as an absolute URL (e.g. from `Router.url`, which
    // always has a leading slash) or as a bare route segment. Normalize
    // before checking for an existing tenant prefix, otherwise
    // `/tenant/games/library` isn't recognized as already-prefixed and gets
    // prefixed again into the malformed `tenant//tenant/games/library`.
    const normalizedPath = path.startsWith('/') ? path.slice(1) : path;
    if (normalizedPath === tenant || normalizedPath.startsWith(`${tenant}/`)) {
      return normalizedPath;
    }

    return [tenant, normalizedPath].join('/');
  }

  public navigateGlobal(path: string | string[]) {
    const segments = Array.isArray(path) ? path : [path];
    return this.router.navigate(['/', ...segments]);
  }

   public navigateByUrl(path: string) {
    return this.router.navigateByUrl(this.buildTenantUrl(path));
  }
}
