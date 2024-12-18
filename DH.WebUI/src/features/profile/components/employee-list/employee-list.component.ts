import { Component, OnDestroy } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../../../entities/auth/auth.service';
import { NAV_ITEM_LABELS } from '../../../../shared/models/nav-items-labels.const';
import { MenuTabsService } from '../../../../shared/services/menu-tabs.service';
import { IGameListResult } from '../../../../entities/games/models/game-list.model';
import { GamesService } from '../../../../entities/games/api/games.service';

@Component({
  selector: 'app-profile',
  templateUrl: 'employee-list.component.html',
  styleUrl: 'employee-list.component.scss',
})
export class EmployeeListComponent implements OnDestroy {
      public games: IGameListResult[] = [];
    
  public employees: any;
  constructor(
    private readonly menuTabsService: MenuTabsService,
    private readonly authService: AuthService,
        private readonly gameService: GamesService,
    private readonly router: Router
  ) {
    this.gameService.getList().subscribe({
        next: (gameList) => (this.games = gameList ?? []),
        error: (error) => {
          console.log(error);
        },
      });
    this.menuTabsService.setActive(NAV_ITEM_LABELS.PROFILE);
  }

  public ngOnDestroy(): void {
    this.menuTabsService.resetData();
  }

  public onAdd():void{

  }

  public onBack():void{
    this.router.navigateByUrl("profile")
  }
}
