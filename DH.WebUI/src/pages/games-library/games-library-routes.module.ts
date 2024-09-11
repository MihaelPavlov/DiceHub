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
import { ReservationListComponent } from '../../features/games-library/components/reservation-list/page/reservation-list.component';
import { QrCodeComponent } from '../../features/games-library/test/qr-code.component';

const routes: Routes = [
  {
    path: '',
    component: GameNavigationComponent,
    children: [
      {
        path: 'library',
        component: GamesLibraryComponent,
      },
      {
        path: 'library/:id',
        component: GamesLibraryComponent,
      },
      {
        path: 'categories',
        component: GameCategoriesComponent,
      },
      {
        path: 'new',
        component: NewGameListComponent,
      },
    ],
  },
  {
    path: 'qr-code',
    component: QrCodeComponent,
  },
  {
    path: 'add',
    component: AddUpdateGameComponent,
  },
  {
    path: ':id/update',
    component: AddUpdateGameComponent,
  },
  {
    path: 'add-existing-game',
    component: AddUpdateGameComponent,
  },
  {
    path: 'reservations',
    component: ReservationListComponent,
  },
  {
    path: ':id/add-existing-game',
    component: AddUpdateGameComponent,
  },
  {
    path: ':id/details',
    component: GameDetailsComponent,
  },
  {
    path: ':id/availability',
    component: GameAvailabilityComponent,
  },
  {
    path: ':id/reviews',
    component: GameReviewsComponent,
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class GamesLibraryRoutingModule {}
