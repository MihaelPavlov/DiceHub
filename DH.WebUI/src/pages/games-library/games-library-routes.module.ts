import { RouterModule, Routes } from '@angular/router';
import { GamesLibraryComponent } from './page/games-library.component';
import { GameDetailsComponent } from './components/game-details/page/game-details.component';
import { GameAvailabilityComponent } from './components/game-availability/page/game-availability.component';
import { GameReviewsComponent } from './components/game-reviews/page/game-reviews.component';
import { NgModule } from '@angular/core';

const routes: Routes = [
  {
    path: 'library',
    component: GamesLibraryComponent,
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
