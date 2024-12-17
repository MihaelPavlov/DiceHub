import { Injectable } from '@angular/core';
import {
  Router,
  ActivatedRouteSnapshot,
  RouterStateSnapshot,
  UrlTree,
} from '@angular/router';
import { Observable } from 'rxjs';
import { AuthService } from '../../../entities/auth/auth.service';
import { UserRole } from '../../../entities/auth/enums/roles.enum';

@Injectable({
  providedIn: 'root',
})
export class SettingsUserAccessGuard {
  constructor(
    private readonly authService: AuthService,
    private readonly router: Router
  ) {}

  public canActivate(
    _: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): Observable<boolean> | boolean | Promise<boolean> | UrlTree {
    if (
      this.authService.getUser?.role === UserRole.Staff ||
      this.authService.getUser?.role === UserRole.User
    ) {
      return true;
    }

    return this.router.parseUrl('profile/settings');
  }
}
