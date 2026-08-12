import { Injectable } from '@angular/core';
import { CanActivate, Router, UrlTree } from '@angular/router';
import { JwtHelperService } from '@auth0/angular-jwt';
import { AuthService } from '../../entities/auth/auth.service';
import { UserRole } from '../../entities/auth/enums/roles.enum';
import { ROUTE } from '../configs/route.config';

@Injectable({ providedIn: 'root' })
export class SuperAdminGuard implements CanActivate {
  constructor(
    private readonly router: Router,
    private readonly jwtHelper: JwtHelperService,
    private readonly authService: AuthService
  ) {}

  public canActivate(): boolean | UrlTree {
    const token = localStorage.getItem('jwt');

    if (!token || this.jwtHelper.isTokenExpired(token)) {
      return this.router.parseUrl(`/${ROUTE.ADMIN.LOGIN}`);
    }

    const user = this.authService.getUser;
    if (!user || user.role !== UserRole.SuperAdmin) {
      return this.router.parseUrl(`/${ROUTE.ADMIN.LOGIN}`);
    }

    return true;
  }
}
