import { Injectable } from '@angular/core';

/**
 * Single source of truth for the auth tokens across web, installed PWA and the
 * Capacitor Android WebView.
 *
 * "Remember me" chooses which Web Storage the tokens live in:
 *  - true  -> localStorage:   survives a browser restart / PWA relaunch / full
 *             app kill. The user stays signed in until the refresh token is
 *             rejected server-side or they log out.
 *  - false -> sessionStorage: dropped when the tab, the installed-PWA window or
 *             the Android WebView is destroyed, i.e. the user is signed out once
 *             they fully quit. It still survives backgrounding / reloads within
 *             the same session.
 *
 * `authPersistent` is always kept in localStorage so that on the next launch we
 * know which store to read the tokens (and last route) from. It defaults to
 * "persistent" when absent, so users already signed in before this change keep
 * their session.
 */
@Injectable({ providedIn: 'root' })
export class AuthTokenService {
  private static readonly AccessKey = 'jwt';
  private static readonly RefreshKey = 'refreshToken';
  private static readonly LastRouteKey = 'lastRoute';
  private static readonly PersistentKey = 'authPersistent';

  /** Persist the tokens after a successful login. */
  public setTokens(
    accessToken: string,
    refreshToken: string,
    remember: boolean
  ): void {
    this.removeFromBoth(AuthTokenService.AccessKey);
    this.removeFromBoth(AuthTokenService.RefreshKey);

    const store = remember ? localStorage : sessionStorage;
    this.safeSet(store, AuthTokenService.AccessKey, accessToken);
    this.safeSet(store, AuthTokenService.RefreshKey, refreshToken);
    this.safeSet(
      localStorage,
      AuthTokenService.PersistentKey,
      remember ? 'true' : 'false'
    );
  }

  /** Replace the tokens after a refresh, keeping the current persistence choice. */
  public updateTokens(accessToken: string, refreshToken: string): void {
    const store = this.isPersistent() ? localStorage : sessionStorage;
    this.safeSet(store, AuthTokenService.AccessKey, accessToken);
    this.safeSet(store, AuthTokenService.RefreshKey, refreshToken);
  }

  public getToken(): string | null {
    return this.readFromEither(AuthTokenService.AccessKey);
  }

  public getRefreshToken(): string | null {
    return this.readFromEither(AuthTokenService.RefreshKey);
  }

  /** True unless the user explicitly unticked "remember me" on the last login. */
  public isPersistent(): boolean {
    return (
      this.safeGet(localStorage, AuthTokenService.PersistentKey) !== 'false'
    );
  }

  public setLastRoute(url: string): void {
    const store = this.isPersistent() ? localStorage : sessionStorage;
    this.safeSet(store, AuthTokenService.LastRouteKey, url);
  }

  public getLastRoute(): string | null {
    return this.readFromEither(AuthTokenService.LastRouteKey);
  }

  public clearLastRoute(): void {
    this.removeFromBoth(AuthTokenService.LastRouteKey);
  }

  public clearToken(): void {
    this.removeFromBoth(AuthTokenService.AccessKey);
    this.removeFromBoth(AuthTokenService.RefreshKey);
    this.removeFromBoth(AuthTokenService.LastRouteKey);
    this.safeRemove(localStorage, AuthTokenService.PersistentKey);
  }

  private readFromEither(key: string): string | null {
    return this.safeGet(sessionStorage, key) ?? this.safeGet(localStorage, key);
  }

  private removeFromBoth(key: string): void {
    this.safeRemove(localStorage, key);
    this.safeRemove(sessionStorage, key);
  }

  private safeGet(store: Storage, key: string): string | null {
    try {
      return store.getItem(key);
    } catch {
      return null;
    }
  }

  private safeSet(store: Storage, key: string, value: string): void {
    try {
      store.setItem(key, value);
    } catch {
      /* storage unavailable (private mode / quota) - non-fatal */
    }
  }

  private safeRemove(store: Storage, key: string): void {
    try {
      store.removeItem(key);
    } catch {
      /* non-fatal */
    }
  }
}
