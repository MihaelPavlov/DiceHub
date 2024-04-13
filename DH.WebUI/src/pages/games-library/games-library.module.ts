import { NgModule } from '@angular/core';
import { GamesLibraryComponent } from './page/games-library.component';
import { SharedModule } from '../../shared/shared.module';
import { HeaderModule } from '../../widgets/header/header.module';
import { NavBarModule } from '../../widgets/nav-bar/nav-bar.module';

@NgModule({
  declarations: [GamesLibraryComponent],
  exports: [GamesLibraryComponent],
  providers: [],
  imports: [SharedModule, HeaderModule, NavBarModule],
})
export class GamesLibraryModule {}
