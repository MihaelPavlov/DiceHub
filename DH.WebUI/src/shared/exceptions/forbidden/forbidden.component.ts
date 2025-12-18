import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { LanguageService } from '../../services/language.service';
import { AuthService } from '../../../entities/auth/auth.service';
import { ExceptionBaseComponent } from '../base/exception-base.component';
import { TenantRouter } from '../../helpers/tenant-router';
import { TenantContextService } from '../../services/tenant-context.service';

@Component({
  selector: 'app-forbidden',
  templateUrl: 'forbidden.component.html',
  styleUrls: ['forbidden.component.scss'],
  standalone: false,
})
export class ForbiddenComponent extends ExceptionBaseComponent {
  protected imageCode = '403';

  constructor(
    router: Router,
    authService: AuthService,
    languageService: LanguageService,
    tenantRouter: TenantRouter,
    tenantContextService: TenantContextService
  ) {
    super(
      router,
      authService,
      languageService,
      tenantRouter,
      tenantContextService
    );
  }
}
