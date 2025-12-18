import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../../entities/auth/auth.service';
import { LanguageService } from '../../services/language.service';
import { ExceptionBaseComponent } from '../base/exception-base.component';
import { TenantRouter } from '../../helpers/tenant-router';
import { TenantContextService } from '../../services/tenant-context.service';

@Component({
  selector: 'app-unauthorized',
  templateUrl: 'unauthorized.component.html',
  styleUrls: ['unauthorized.component.scss'],
  standalone: false,
})
export class UnauthorizedComponent extends ExceptionBaseComponent {
  protected imageCode = '401';

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
