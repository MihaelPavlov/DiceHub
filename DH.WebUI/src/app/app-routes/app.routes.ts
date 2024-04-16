import { Routes } from '@angular/router';
import { AppComponent } from '../app-component/app.component';

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
      {
        path: 'events',
        loadChildren: () =>
          import('../../pages/events-library/events-library.module').then(
            (m) => m.EventsLibraryModule
          ),
      },
      {
        path: 'meeples',
        loadChildren: () =>
          import('../../pages/find-meeple-management/find-meeple-management.module').then(
            (m) => m.FindMeepleMamagementModule
          ),
      },
    ],
  },
];
