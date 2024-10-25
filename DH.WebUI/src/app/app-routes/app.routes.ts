import { Routes } from '@angular/router';
import { AppComponent } from '../app-component/app.component';
import { LoginComponent } from '../../pages/login/page/login.component';
import { AuthGuard } from '../../shared/guards/auth.guard';
import { EventUserAccessGuard } from '../../shared/guards/event-user.guard';
import { EventAdminAccessGuard } from '../../shared/guards/event-admin-access.guard';
import { ChallengeAdminAccessGuard } from '../../shared/guards/challenge-admin-acces.guard';
import { ChallengeUserAccessGuard } from '../../shared/guards/challenge-user.guard';
import { RegisterComponent } from '../../pages/register/page/register.component';
import { ProfileComponent } from '../../pages/profile/page/profile.component';

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
        canActivate: [EventUserAccessGuard],
      },
      {
        path: 'admin-events',
        loadChildren: () =>
          import(
            '../../pages/admin-event-management/admin-event-management.module'
          ).then((m) => m.AdminEventManagementModule),
        canActivate: [EventAdminAccessGuard],
      },
      {
        path: 'meeples',
        loadChildren: () =>
          import(
            '../../pages/find-meeple-management/find-meeple-management.module'
          ).then((m) => m.FindMeepleManagementModule),
      },
      {
        path: 'challenges',
        loadChildren: () =>
          import(
            '../../pages/challenges-management/challenges-management.module'
          ).then((m) => m.ChallengesManagementModule),
        canActivate: [ChallengeUserAccessGuard],
      },
      {
        path: 'admin-challenges',
        loadChildren: () =>
          import(
            '../../pages/admin-challenges-management/admin-challenges-management.module'
          ).then((m) => m.AdminChallengesManagementModule),
        canActivate: [ChallengeAdminAccessGuard],
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
