import { NgModule } from '@angular/core';
import { GamesLibraryComponent } from './page/games-library.component';
import { SharedModule } from '../../shared/shared.module';
import { HeaderModule } from '../../widgets/header/header.module';
import { NavBarModule } from '../../widgets/nav-bar/nav-bar.module';
import { GameDetailsComponent } from './components/game-details/page/game-details.component';
import { GameAvailabilityComponent } from './components/game-availability/page/game-availability.component';
import { GameLayoutComponent } from './components/game-layout/page/game-layout.component';
import { GameReviewsComponent } from './components/game-reviews/page/game-reviews.component';
import { GamesLibraryRoutingModule } from './games-library-routes.module';

@NgModule({
  declarations: [
    GamesLibraryComponent,
    GameDetailsComponent,
    GameAvailabilityComponent,
    GameLayoutComponent,
    GameReviewsComponent,
  ],
  exports: [GamesLibraryComponent],
  providers: [],
  imports: [
    SharedModule,
    HeaderModule,
    NavBarModule,
    GamesLibraryRoutingModule,
  ],
})
export class GamesLibraryModule {}
