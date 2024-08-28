import {
  ActivatedRouteSnapshot,
  Router,
  RouterStateSnapshot,
} from '@angular/router';
import { Injectable } from '@angular/core';
import { AuthService } from '../../entities/auth/auth.service';
import { UserRole } from '../../entities/auth/enums/roles.enum';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class EventAdminAccessGuard {
  constructor(
    private readonly authService: AuthService,
    private readonly router: Router
  ) {}

  public canActivate(
    _: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): Observable<boolean> | boolean | Promise<boolean> {
    if (this.authService.getUser?.role !== UserRole.User) {
      return true;
    }

    this.router.navigateByUrl('events');
    return false;
  }
}
