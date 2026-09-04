import { Component } from '@angular/core';

/**
 * Static, unlinked privacy policy page. It is intentionally not referenced from
 * any menu, footer or navigation - the only way to reach it is the direct
 * `/privacy-policy` URL, which is what Google Play Console requires for the
 * "Privacy policy" field before internal/closed testing and production.
 */
@Component({
  selector: 'app-privacy-policy',
  templateUrl: 'privacy-policy.component.html',
  styleUrl: 'privacy-policy.component.scss',
  standalone: false,
})
export class PrivacyPolicyComponent {
  /** Bumped whenever the policy text below is materially changed. */
  public readonly lastUpdated = 'September 4, 2026';
  public readonly contactEmail = 'dicehubapp@gmail.com';
}
