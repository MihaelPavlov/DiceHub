import {
  ActivatedRouteSnapshot,
  Router,
  RouterStateSnapshot,
  UrlTree,
} from '@angular/router';
import { Injectable } from '@angular/core';
import { AuthService } from '../../entities/auth/auth.service';
import { UserRole } from '../../entities/auth/enums/roles.enum';
import { Observable } from 'rxjs';
import { FULL_ROUTE } from '../configs/route.config';
import { TenantRouter } from '../helpers/tenant-router';

@Injectable({
  providedIn: 'root',
})
export class EventUserAccessGuard {
  constructor(
    private readonly authService: AuthService,
    private readonly router: Router,
    private readonly tenantRouter: TenantRouter
  ) {}

  public canActivate(
    _: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): Observable<boolean> | boolean | Promise<boolean> | UrlTree {
    if (this.authService.getUser?.role !== UserRole.User) {
      return this.router.parseUrl(
        this.tenantRouter.buildTenantUrl(FULL_ROUTE.EVENTS.ADMIN.CORE)
      );
    }

    return true;
  }
}
