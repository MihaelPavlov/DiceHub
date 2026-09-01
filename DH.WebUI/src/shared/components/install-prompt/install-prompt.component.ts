import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { Capacitor } from '@capacitor/core';

type InstallPlatform = 'android' | 'ios' | 'desktop';

interface IBeforeInstallPromptEvent extends Event {
  prompt(): Promise<void>;
  userChoice: Promise<{ outcome: 'accepted' | 'dismissed' }>;
}

@Component({
  selector: 'app-install-prompt',
  templateUrl: './install-prompt.component.html',
  styleUrl: './install-prompt.component.scss',
  standalone: false,
})
export class InstallPromptComponent implements OnInit {
  private static readonly DismissKey = 'installPromptDismissedAt';
  private static readonly DismissDurationMs = 14 * 24 * 60 * 60 * 1000;
  private static readonly HowToInstallRoute = '/instructions/how_to_install';
  public static readonly PlayStoreUrl =
    'https://play.google.com/store/apps/details?id=com.dicehubs.app';

  public isVisible = false;
  public platform: InstallPlatform = 'desktop';
  /** Shown below the buttons once "Install app" is clicked but no native install prompt was available. */
  public showInstallHelp = false;
  public readonly playStoreUrl = InstallPromptComponent.PlayStoreUrl;

  private deferredPrompt: IBeforeInstallPromptEvent | null = null;

  constructor(private readonly router: Router) {}

  public ngOnInit(): void {
    if (this.isAlreadyInstalledOrNative() || this.isRecentlyDismissed()) {
      return;
    }

    this.platform = this.detectPlatform();

    // Chrome/Edge only fire this if/when they decide the page is installable;
    // captured opportunistically so "Install app" can use it if it has arrived
    // by the time the user clicks, without gating the whole banner on it.
    window.addEventListener('beforeinstallprompt', (event: Event) => {
      event.preventDefault();
      this.deferredPrompt = event as IBeforeInstallPromptEvent;
    });

    this.registerControllingServiceWorker();

    this.isVisible = true;
  }

  /**
   * beforeinstallprompt only fires once Chrome sees an active service worker
   * whose scope actually covers the current page. The app's only existing
   * service worker (firebase-messaging-sw.js, registered by MessagingService)
   * is deliberately scoped to a fake '/firebase-cloud-messaging-push-scope'
   * path for push handling, so it never controls '/login' or '/register' -
   * this page was never installable in the first place. Register the same
   * script again at root scope, purely so it controls this page; it has no
   * fetch handler, so it doesn't intercept or cache anything.
   */
  private registerControllingServiceWorker(): void {
    if (!('serviceWorker' in navigator)) {
      return;
    }

    navigator.serviceWorker
      .register('/firebase-messaging-sw.js', { scope: '/' })
      .catch(() => {
        // Installability just won't be available this visit; the Play
        // Store link and manual-install hint still work regardless.
      });
  }

  public dismiss(): void {
    this.isVisible = false;
    localStorage.setItem(InstallPromptComponent.DismissKey, Date.now().toString());
  }

  public async installPwa(): Promise<void> {
    if (!this.deferredPrompt) {
      // No native install prompt available (iOS Safari, or Chrome hasn't
      // decided the page is installable yet) - offer the illustrated guide
      // as a follow-up rather than yanking the user off this page unasked.
      this.showInstallHelp = true;
      return;
    }

    const prompt = this.deferredPrompt;
    this.deferredPrompt = null;

    await prompt.prompt();
    const choice = await prompt.userChoice;
    if (choice.outcome === 'accepted') {
      this.isVisible = false;
    } else {
      this.showInstallHelp = true;
    }
  }

  public goToInstallInstructions(): void {
    this.router.navigateByUrl(InstallPromptComponent.HowToInstallRoute);
  }

  private isAlreadyInstalledOrNative(): boolean {
    if (Capacitor.isNativePlatform()) {
      return true;
    }

    if (window.matchMedia('(display-mode: standalone)').matches) {
      return true;
    }

    // iOS Safari's standalone-PWA flag - not part of the standard Navigator type.
    return (navigator as Navigator & { standalone?: boolean }).standalone === true;
  }

  private isRecentlyDismissed(): boolean {
    const raw = localStorage.getItem(InstallPromptComponent.DismissKey);
    if (!raw) {
      return false;
    }

    return Date.now() - Number(raw) < InstallPromptComponent.DismissDurationMs;
  }

  private detectPlatform(): InstallPlatform {
    const ua = navigator.userAgent.toLowerCase();
    if (/android/.test(ua)) {
      return 'android';
    }
    if (/iphone|ipad|ipod/.test(ua)) {
      return 'ios';
    }
    return 'desktop';
  }
}
