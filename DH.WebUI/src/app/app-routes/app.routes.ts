import { Routes } from '@angular/router';
import { Component } from '@angular/core';
import { GamesLibraryComponent } from '../../pages/games-library/page/games-library.component';
import { AppComponent } from '../app-component/app.component';

@Component({
  selector: 'app-home',
  template: `
  
  <span>home</span>`,
})
export class HomeComponent  {

}

export const ROUTES: Routes = [
  {
    path: '',
    component: AppComponent,
    children: [
      {
        path: 'games',
        loadChildren: () =>
          import('../../pages/games-library/games-library.module').then(
            (m) => m.GamesLibraryModule
          ),
      },
      // {
      //   path: 'details/:id',
      //   loadChildren: () =>
      //     import('../../pages/game-details/game-details.module').then(
      //       (m) => m.GameDetailsModule
      //     ),
      // },
    ],
  },
];
