import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { ControlsMenuComponent } from '../../../../shared/components/menu/controls-menu.component';

@Component({
  selector: 'leaderboard-challenges',
  templateUrl: 'leaderboard-challenges.component.html',
  styleUrl: 'leaderboard-challenges.component.scss',
})
export class LeaderboardChallengesComponent {
  users: any[] = [];

  constructor(private readonly router: Router) {}

  public backNavigateBtn(): void {
    this.router.navigateByUrl('profile');
  }

   public showMenu(event: MouseEvent, controlMenu: ControlsMenuComponent): void {
      event.stopPropagation();
      controlMenu.toggleMenu();
    }
}
