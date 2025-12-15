import { RouterModule, Routes } from '@angular/router';
import { GamesLibraryComponent } from './page/games-library.component';
import { NgModule } from '@angular/core';
import { GameDetailsComponent } from '../../features/games-library/components/game-details/page/game-details.component';
import { GameAvailabilityComponent } from '../../features/games-library/components/game-availability/page/game-availability.component';
import { GameReviewsComponent } from '../../features/games-library/components/game-reviews/page/game-reviews.component';
import { GameCategoriesComponent } from '../../features/games-library/components/game-categories/page/game-categories.component';
import { GameNavigationComponent } from '../../features/games-library/components/game-navigation/page/game-navigation.component';
import { NewGameListComponent } from '../../features/games-library/components/new-game-list/page/new-game-list.component';
import { AddUpdateGameComponent } from '../../features/games-library/components/add-update-game/page/add-update-game.component';
import { AuthGuard } from '../../shared/guards/auth.guard';

const routes: Routes = [
  {
    path: '',
    component: GameNavigationComponent,
    canActivate: [AuthGuard],
    children: [
      {
        path: 'library',
        component: GamesLibraryComponent,
        canActivate: [AuthGuard],
      },
      {
        path: 'library/:id',
        component: GamesLibraryComponent,
        canActivate: [AuthGuard],
      },
      {
        path: 'categories',
        component: GameCategoriesComponent,
        canActivate: [AuthGuard],
      },
      {
        path: 'new',
        component: NewGameListComponent,
        canActivate: [AuthGuard],
      },
    ],
  },
  {
    path: 'add',
    component: AddUpdateGameComponent,
    canActivate: [AuthGuard],
  },
  {
    path: ':id/update',
    component: AddUpdateGameComponent,
    canActivate: [AuthGuard],
  },
  {
    path: 'add-existing-game',
    component: AddUpdateGameComponent,
    canActivate: [AuthGuard],
  },
  {
    path: ':id/add-existing-game',
    component: AddUpdateGameComponent,
    canActivate: [AuthGuard],
  },
  {
    path: ':id/details',
    component: GameDetailsComponent,
    canActivate: [AuthGuard],
  },
  {
    path: ':id/availability',
    component: GameAvailabilityComponent,
    canActivate: [AuthGuard],
  },
  {
    path: ':id/reviews',
    component: GameReviewsComponent,
    canActivate: [AuthGuard],
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class GamesLibraryRoutingModule {}
