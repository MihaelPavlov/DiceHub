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
import { ReactiveFormsModule } from '@angular/forms';
import {
  MatDialogActions,
  MatDialogClose,
  MatDialogContent,
  MatDialogModule,
  MatDialogTitle,
} from '@angular/material/dialog';
import { GameReviewConfirmDeleteDialog } from '../../features/games-library/components/game-reviews/components/game-review-confirm-delete/game-review-confirm-delete.component';
import { ButtonModule } from '../../widgets/button/button.module';

@NgModule({
  declarations: [
    GamesLibraryComponent,
    GameDetailsComponent,
    GameAvailabilityComponent,
    GameLayoutComponent,
    GameReviewsComponent,
    GameReviewConfirmDeleteDialog,
  ],
  exports: [GamesLibraryComponent],
  providers: [],
  imports: [
    SharedModule,
    HttpClientModule,
    HeaderModule,
    NavBarModule,
    GamesLibraryRoutingModule,
    ReactiveFormsModule,
    MatDialogTitle,
    MatDialogContent,
    MatDialogActions,
    MatDialogClose,
    ButtonModule,
    MatDialogModule
  ],
})
export class GamesLibraryModule {}
