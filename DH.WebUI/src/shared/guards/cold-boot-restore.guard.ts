import { Injectable } from '@angular/core';
import { CanActivate, Router, UrlTree } from '@angular/router';
import { Capacitor } from '@capacitor/core';
import { AuthTokenService } from '../services/auth-token.service';

/**
 * Redirects the app's very first navigation (native cold boot only) straight to the
 * user's last route, before the landing route ever activates - a CanActivate guard
 * runs before the routed component is instantiated, so unlike a post-NavigationEnd
 * redirect this produces no visible flash of the landing page first. Only ever
 * applies once per process lifetime: subsequent in-app navigations to '/' (e.g. the
 * logo/back-to-landing action) must show the real landing page.
 */
@Injectable({ providedIn: 'root' })
export class ColdBootRestoreGuard implements CanActivate {
  private hasChecked = false;

  constructor(
    private readonly router: Router,
    private readonly authTokenService: AuthTokenService
  ) {}

  public canActivate(): boolean | UrlTree {
    if (this.hasChecked || !Capacitor.isNativePlatform()) {
      return true;
    }
    this.hasChecked = true;

    const lastRoute = this.authTokenService.getLastRoute();
    if (!lastRoute || lastRoute === '/') {
      return true;
    }

    // Don't restore straight onto an error page, and don't trust a route that
    // won't parse (e.g. saved by an older app version) - clear it and show
    // landing instead of bouncing the user into a 404 on every launch.
    const errorSegments = [
      '/not-found',
      '/unauthorized',
      '/forbidden',
      '/server-error',
    ];
    if (errorSegments.some((segment) => lastRoute.includes(segment))) {
      this.authTokenService.clearLastRoute();
      return true;
    }

    try {
      return this.router.parseUrl(lastRoute);
    } catch {
      this.authTokenService.clearLastRoute();
      return true;
    }
  }
}
