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

@Injectable({
  providedIn: 'root',
})
export class AuthRedirectGuard {
  constructor(
    private readonly router: Router,
    private readonly tenantRouter: TenantRouter,
    private readonly jwtHelper: JwtHelperService
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
    console.log('auth-redirect.guard.ts');

    if (token && !this.jwtHelper.isTokenExpired(token)) {
      const tenantUrl = this.tenantRouter.buildTenantUrl(
        FULL_ROUTE.GAMES.LIBRARY
      );
      return this.router.parseUrl(tenantUrl);
    }
    return of(true);
  }
}
