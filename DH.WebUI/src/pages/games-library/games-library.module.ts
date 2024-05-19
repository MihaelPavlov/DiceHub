import { NgModule } from '@angular/core';
import { GamesLibraryComponent } from './page/games-library.component';
import { SharedModule } from '../../shared/shared.module';
import { HeaderModule } from '../../widgets/header/header.module';
import { NavBarModule } from '../../widgets/nav-bar/nav-bar.module';
import { GamesLibraryRoutingModule } from './games-library-routes.module';
import { GameDetailsComponent } from '../../features/games-library/components/game-details/page/game-details.component';
import { GameAvailabilityComponent } from '../../features/games-library/components/game-availability/page/game-availability.component';
import { GameLayoutComponent } from '../../features/games-library/components/game-layout/page/game-layout.component';
import { GameReviewsComponent } from '../../features/games-library/components/game-reviews/page/game-reviews.component';
import { HttpClientModule } from '@angular/common/http';
import { JWT_OPTIONS, JwtHelperService, JwtModule } from '@auth0/angular-jwt';
import { AuthGuard } from './auth.guard';

@NgModule({
  declarations: [
    GamesLibraryComponent,
    GameDetailsComponent,
    GameAvailabilityComponent,
    GameLayoutComponent,
    GameReviewsComponent,
  ],
  exports: [GamesLibraryComponent],
  providers: [    AuthGuard,
    { provide: JWT_OPTIONS, useValue: JWT_OPTIONS },
    JwtHelperService],
  imports: [
    SharedModule,
    HttpClientModule,
    HeaderModule,
    NavBarModule,
    GamesLibraryRoutingModule,
    JwtModule,
  ],
})
export class GamesLibraryModule {}
