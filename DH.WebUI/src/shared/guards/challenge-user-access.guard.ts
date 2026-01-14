import { IUserInfo } from './../../entities/auth/models/user-info.model';
import {
  ActivatedRouteSnapshot,
  Router,
  RouterStateSnapshot,
  UrlTree,
} from '@angular/router';
import { Injectable } from '@angular/core';
import { AuthService } from '../../entities/auth/auth.service';
import { UserRole } from '../../entities/auth/enums/roles.enum';
import {
  catchError,
  filter,
  from,
  map,
  Observable,
  of,
  switchMap,
  take,
} from 'rxjs';
import { FULL_ROUTE, ROUTE } from '../configs/route.config';
import { TenantRouter } from '../helpers/tenant-router';

@Injectable({
  providedIn: 'root',
})
export class ChallengeUserAccessGuard {
  constructor(
    private readonly authService: AuthService,
    private readonly router: Router,
    private readonly tenantRouter: TenantRouter
  ) {}

  canActivate(
    _: ActivatedRouteSnapshot,
    __: RouterStateSnapshot
  ): Observable<boolean | UrlTree> {
    const cachedUser = this.authService.getUser; // could be undefined | null | IUserInfo

    // If user info is already known, short-circuit (no extra network call)
    if (cachedUser !== undefined) {
      if (!cachedUser) {
        return of(
          this.router.parseUrl(this.tenantRouter.buildTenantUrl(ROUTE.LOGIN))
        );
      }
      if (cachedUser.role !== UserRole.User) {
        return of(this.router.parseUrl(FULL_ROUTE.CHALLENGES.ADMIN_LIST));
      }
      return of(true);
    }

    // Otherwise wait for the APP initializer / userinfo$() to complete
    return from(this.authService.userinfo$()).pipe(
      // after userinfo$ resolves, read the BehaviorSubject/Observable
      switchMap(() => this.authService.userInfoSubject$),
      // skip the initial undefined value if the subject uses undefined for "not loaded yet"
      filter((u): u is IUserInfo | null => typeof u !== 'undefined'),
      take(1),
      map((user) => {
        if (!user)
          return this.router.parseUrl(
            this.tenantRouter.buildTenantUrl(ROUTE.LOGIN)
          );
        if (user.role !== UserRole.User)
          return this.router.parseUrl(FULL_ROUTE.CHALLENGES.ADMIN_LIST);
        return true;
      }),
      catchError(() =>
        of(this.router.parseUrl(this.tenantRouter.buildTenantUrl(ROUTE.LOGIN)))
      )
    );
  }
}
