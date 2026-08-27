import { Component, OnInit } from '@angular/core';
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
  public static readonly PlayStoreUrl =
    'https://play.google.com/store/apps/details?id=com.dicehubs.app';

  public isVisible = false;
  public platform: InstallPlatform = 'desktop';
  /** Shown once "Install app" is clicked but no native install prompt was available. */
  public showManualInstallHint = false;
  public readonly playStoreUrl = InstallPromptComponent.PlayStoreUrl;

  private deferredPrompt: IBeforeInstallPromptEvent | null = null;

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

    this.isVisible = true;
  }

  public dismiss(): void {
    this.isVisible = false;
    localStorage.setItem(InstallPromptComponent.DismissKey, Date.now().toString());
  }

  public async installPwa(): Promise<void> {
    if (!this.deferredPrompt) {
      // No native install prompt available (iOS Safari, or Chrome hasn't
      // decided the page is installable yet) - show manual steps instead.
      this.showManualInstallHint = true;
      return;
    }

    const prompt = this.deferredPrompt;
    this.deferredPrompt = null;

    await prompt.prompt();
    const choice = await prompt.userChoice;
    if (choice.outcome === 'accepted') {
      this.isVisible = false;
    }
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
