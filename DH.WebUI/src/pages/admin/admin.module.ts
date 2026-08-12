import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { SharedModule } from '../../shared/shared.module';
import { ADMIN_ROUTES } from './admin.routes';
import { AdminLoginComponent } from './pages/admin-login/admin-login.component';
import { ProvisionTenantComponent } from './pages/provision-tenant/provision-tenant.component';

@NgModule({
  declarations: [AdminLoginComponent, ProvisionTenantComponent],
  imports: [SharedModule, RouterModule.forChild(ADMIN_ROUTES)],
})
export class AdminModule {}
