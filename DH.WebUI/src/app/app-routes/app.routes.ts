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
          import('../../pages/event-library/events-library.module').then(
            (m) => m.EventsLibraryModule
          ),
      },
    ],
  },
];
