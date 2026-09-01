import { Injectable } from '@angular/core';
import {
  CanActivate,
  ActivatedRouteSnapshot,
  Router,
  UrlTree,
} from '@angular/router';
import { TenantContextService } from '../services/tenant-context.service';
import { ROUTE } from '../configs/route.config';
import { TenantService } from '../services/tenant.service';
import { of, map, catchError, Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class TenantGuard implements CanActivate {
  constructor(
    private readonly tenantContextService: TenantContextService,
    private readonly tenantService: TenantService,
    private readonly router: Router
  ) {}

  public canActivate(
    route: ActivatedRouteSnapshot
  ): Observable<boolean | UrlTree> {
    const tenantId = route.paramMap.get('tenant');

    if (!tenantId) {
      // No tenant param → redirect to choose club
      return of(this.router.parseUrl(ROUTE.CHOOSE_CLUB));
    }

    // Validate tenant via backend
    return this.tenantService.validateTenant(tenantId).pipe(
      map((isValid) => {
        if (isValid) {
          // Only set context if backend confirms tenant exists
          this.tenantContextService.tenantId = tenantId;
          return true;
        } else {
          // Invalid tenant → redirect
          return this.router.parseUrl(ROUTE.CHOOSE_CLUB);
        }
      }),
      catchError(() => of(this.router.parseUrl(ROUTE.CHOOSE_CLUB)))
    );
  }
}
