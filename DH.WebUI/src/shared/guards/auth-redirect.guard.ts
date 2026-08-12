import { Injectable } from '@angular/core';
import {
  ActivatedRouteSnapshot,
  Router,
  RouterStateSnapshot,
  UrlTree,
} from '@angular/router';
import { Observable, of } from 'rxjs';
import { FULL_ROUTE } from '../configs/route.config';
import { JwtHelperService } from '@auth0/angular-jwt';
import { TenantRouter } from '../helpers/tenant-router';
import { TenantContextService } from '../services/tenant-context.service';

@Injectable({
  providedIn: 'root',
})
export class AuthRedirectGuard {
  constructor(
    private readonly router: Router,
    private readonly tenantRouter: TenantRouter,
    private readonly jwtHelper: JwtHelperService,
    private readonly tenantContextService: TenantContextService
  ) {}

  public canActivate(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ):
    | boolean
    | UrlTree
    | Observable<boolean | UrlTree>
    | Promise<boolean | UrlTree> {
    const token = localStorage.getItem('jwt');

    if (token && !this.jwtHelper.isTokenExpired(token)) {
      const user = this.jwtHelper.decodeToken(token);

      if (user?.['tenant_id'] === 'system') {
        return this.router.parseUrl('/admin/applicants');
      }

      if (this.tenantContextService.hasTenant()) {
        const tenantUrl = this.tenantRouter.buildTenantUrl(
          FULL_ROUTE.GAMES.LIBRARY
        );
        return this.router.parseUrl(tenantUrl);
      }
    }
    return of(true);
  }
}
