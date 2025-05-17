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
export class SettingsOwnerAccessGuard {
  constructor(
    private readonly authService: AuthService,
    private readonly router: Router
  ) {}

  public canActivate(
    _: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): Observable<boolean> | boolean | Promise<boolean> | UrlTree {
    if (
      this.authService.getUser?.role === UserRole.Owner ||
      this.authService.getUser?.role === UserRole.SuperAdmin
    ) {
      return true;
    }

    return this.router.parseUrl('profile');
  }
}
