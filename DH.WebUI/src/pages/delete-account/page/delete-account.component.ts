import { Component } from '@angular/core';

/**
 * Static, unlinked account & data deletion page. Not referenced from any menu,
 * footer or navigation - reachable only via the direct `/delete-account` URL,
 * which is what Google Play Console requires for the "Delete account URL" field
 * on the store listing (Data safety -> account deletion).
 */
@Component({
  selector: 'app-delete-account',
  templateUrl: 'delete-account.component.html',
  styleUrl: 'delete-account.component.scss',
  standalone: false,
})
export class DeleteAccountComponent {
  public readonly lastUpdated = 'September 4, 2026';
  public readonly contactEmail = 'dicehubapp@gmail.com';
}
