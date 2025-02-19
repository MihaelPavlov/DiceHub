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

@Injectable({
  providedIn: 'root',
})
export class ChallengeUserAccessGuard {
  constructor(
    private readonly authService: AuthService,
    private readonly router: Router
  ) {}

  public canActivate(
    _: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): Observable<boolean> | boolean | Promise<boolean> | UrlTree {
    if (this.authService.getUser?.role !== UserRole.User) {
      return this.router.parseUrl(FULL_ROUTE.CHALLENGES.ADMIN_LIST);
    }

    return true;
  }
}
