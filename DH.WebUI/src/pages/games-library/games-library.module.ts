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
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import {
  MatDialogActions,
  MatDialogClose,
  MatDialogContent,
  MatDialogModule,
  MatDialogTitle,
} from '@angular/material/dialog';
import { GameReviewConfirmDeleteDialog } from '../../features/games-library/components/game-reviews/components/game-review-confirm-delete/game-review-confirm-delete.component';
import { GameCategoriesComponent } from '../../features/games-library/components/game-categories/page/game-categories.component';
import { GameNavigationComponent } from '../../features/games-library/components/game-navigation/page/game-navigation.component';
import { NewGameListComponent } from '../../features/games-library/components/new-game-list/page/new-game-list.component';
import { AddUpdateGameComponent } from '../../features/games-library/components/add-update-game/page/add-update-game.component';
import { NgSelectModule } from '@ng-select/ng-select';
import { GameConfirmDeleteDialog } from '../../features/games-library/dialogs/game-confirm-delete-dialog/game-confirm-delete.component';
import { ReservationListComponent } from '../../features/games-library/components/reservation-list/page/reservation-list.component';
import { QrCodeComponent } from '../../features/games-library/test/qr-code.component';
import { GameQrCodeListComponent } from '../../features/games-library/components/qr-code-list/page/game-qr-code-list.component';

@NgModule({
  declarations: [
    GamesLibraryComponent,
    GameDetailsComponent,
    GameAvailabilityComponent,
    GameLayoutComponent,
    GameReviewsComponent,
    GameCategoriesComponent,
    GameNavigationComponent,
    NewGameListComponent,
    AddUpdateGameComponent,
    GameReviewConfirmDeleteDialog,
    GameConfirmDeleteDialog,
    ReservationListComponent,
    QrCodeComponent,
    GameQrCodeListComponent
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
    MatDialogModule,
    FormsModule,
    NgSelectModule,
  ],
})
export class GamesLibraryModule {}
