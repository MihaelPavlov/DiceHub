import { Routes } from '@angular/router';
import { GamesLibraryComponent } from '../../pages/games-library/page/games-library.component';

export const ROUTES: Routes = [
  {
    path: 'games',
    component: GamesLibraryComponent,
    children: [
      {
        path: 'library',
        loadChildren: () =>
          import('../../pages/games-library/games-library.module').then(
            (m) => m.GamesLibraryModule
          ),
      },
      // {
      //   path: 'details/:id',
      //   loadChildren: () =>
      //     import('../../pages/games-library/games-library.module').then(
      //       (m) => m.GamesLibraryModule
      //     ),
      // },
    ],
  },
];
