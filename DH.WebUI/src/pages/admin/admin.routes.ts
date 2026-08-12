import { Routes } from '@angular/router';
import { AdminLoginComponent } from './pages/admin-login/admin-login.component';
import { ProvisionTenantComponent } from './pages/provision-tenant/provision-tenant.component';
import { SuperAdminGuard } from '../../shared/guards/super-admin.guard';

export const ADMIN_ROUTES: Routes = [
  { path: '', redirectTo: 'login', pathMatch: 'full' },
  {
    path: 'login',
    component: AdminLoginComponent,
    data: { hideMenu: true },
  },
  {
    path: 'provision',
    component: ProvisionTenantComponent,
    canActivate: [SuperAdminGuard],
    data: { hideMenu: true },
  },
];
