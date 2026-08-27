import { Injectable } from '@angular/core';
import { CanActivate, Router, UrlTree } from '@angular/router';
import { Capacitor } from '@capacitor/core';

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
  private static readonly LastRouteStorageKey = 'lastRoute';
  private hasChecked = false;

  constructor(private readonly router: Router) {}

  public canActivate(): boolean | UrlTree {
    if (this.hasChecked || !Capacitor.isNativePlatform()) {
      return true;
    }
    this.hasChecked = true;

    const lastRoute = localStorage.getItem(ColdBootRestoreGuard.LastRouteStorageKey);
    if (lastRoute && lastRoute !== '/') {
      return this.router.parseUrl(lastRoute);
    }

    return true;
  }
}
