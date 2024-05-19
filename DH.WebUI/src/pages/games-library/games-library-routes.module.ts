import { RouterModule, Routes } from '@angular/router';
import { GamesLibraryComponent } from './page/games-library.component';
import { NgModule } from '@angular/core';
import { GameDetailsComponent } from '../../features/games-library/components/game-details/page/game-details.component';
import { GameAvailabilityComponent } from '../../features/games-library/components/game-availability/page/game-availability.component';
import { GameReviewsComponent } from '../../features/games-library/components/game-reviews/page/game-reviews.component';
import { AuthGuard } from './auth.guard';

const routes: Routes = [
  {
    path: 'library',
    component: GamesLibraryComponent,
    // canActivate: [AuthGuard],
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
