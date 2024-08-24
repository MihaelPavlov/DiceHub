import { Routes } from '@angular/router';
import { AppComponent } from '../app-component/app.component';
import { LoginComponent } from '../../pages/login/page/login.component';
import { AuthGuard } from '../../shared/guards/auth.guard';

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
        canActivate: [AuthGuard],
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
          import(
            '../../pages/find-meeple-management/find-meeple-management.module'
          ).then((m) => m.FindMeepleMamagementModule),
      },
      {
        path: 'challenges',
        loadChildren: () =>
          import(
            '../../pages/challenges-management/challenges-management.module'
          ).then((m) => m.ChallengesManagementModule),
      },
      {
        path: 'space',
        loadChildren: () =>
          import(
            '../../pages/club-space-management/club-space-management.module'
          ).then((m) => m.ClubSpaceManagementModule),
      },
      {
        path: 'admin-space',
        loadChildren: () =>
          import(
            '../../pages/admin-space/admin-space.module'
          ).then((m) => m.AdminSpaceModule),
      },
    ],
  },
  {
    path: 'login',
    component: LoginComponent,
  },
  {
    path:'unauthorized',
    loadChildren: () =>
      import(
        '../../shared/exceptions/unauthorized/unathorized.module'
      ).then((m) => m.UnauthorizedModule),
  },
  {
    path:'forbidden',
    loadChildren: () =>
      import(
        '../../shared/exceptions/forbidden/forbidden.module'
      ).then((m) => m.ForbiddenModule),
  },
  {
    path:'not-found',
    loadChildren: () =>
      import(
        '../../shared/exceptions/not-found/not-found.module'
      ).then((m) => m.NotFoundModule),
  }
];
