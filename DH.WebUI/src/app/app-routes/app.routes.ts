import { Routes } from '@angular/router';
import { AppComponent } from '../app-component/app.component';
import { LoginComponent } from '../../pages/login/page/login.component';
import { AuthGuard } from '../../shared/guards/auth.guard';
import { RegisterComponent } from '../../pages/register/page/register.component';
import { ROUTE } from '../../shared/configs/route.config';

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
        path: ROUTE.EVENTS.CORE,
        loadChildren: () =>
          import('../../pages/events-library/events-library.module').then(
            (m) => m.EventsLibraryModule
          ),
        canActivate: [AuthGuard],
      },
      {
        path: 'meeples',
        loadChildren: () =>
          import(
            '../../pages/find-meeple-management/find-meeple-management.module'
          ).then((m) => m.FindMeepleManagementModule),
        canActivate: [AuthGuard],
      },
      {
        path: ROUTE.CHALLENGES.CORE,
        loadChildren: () =>
          import(
            '../../pages/challenges-management/challenges-management.module'
          ).then((m) => m.ChallengesManagementModule),
        canActivate: [AuthGuard],
      },
      {
        path: 'space',
        loadChildren: () =>
          import(
            '../../pages/club-space-management/club-space-management.module'
          ).then((m) => m.ClubSpaceManagementModule),
        canActivate: [AuthGuard],
      },
      {
        path: 'admin-space',
        loadChildren: () =>
          import('../../pages/admin-space/admin-space.module').then(
            (m) => m.AdminSpaceModule
          ),
      },
      {
        path: 'qr-code-scanner',
        loadChildren: () =>
          import('../../pages/qr-code-scanner/qr-code-scanner.module').then(
            (m) => m.QrCodeScannerModule
          ),
        canActivate: [AuthGuard],
      },
      {
        path: 'profile',
        loadChildren: () =>
          import('../../pages/profile/profile.module').then(
            (m) => m.ProfileModule
          ),
        canActivate: [AuthGuard],
      },
    ],
  },
  {
    path: 'login',
    component: LoginComponent,
  },
  {
    path: 'register',
    component: RegisterComponent,
  },
  {
    path: 'unauthorized',
    loadChildren: () =>
      import('../../shared/exceptions/unauthorized/unauthorized.module').then(
        (m) => m.UnauthorizedModule
      ),
  },
  {
    path: 'forbidden',
    loadChildren: () =>
      import('../../shared/exceptions/forbidden/forbidden.module').then(
        (m) => m.ForbiddenModule
      ),
  },
  {
    path: 'not-found',
    loadChildren: () =>
      import('../../shared/exceptions/not-found/not-found.module').then(
        (m) => m.NotFoundModule
      ),
  },
];
