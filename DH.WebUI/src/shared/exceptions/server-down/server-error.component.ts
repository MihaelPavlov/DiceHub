import { TenantContextService } from './../../services/tenant-context.service';
import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../../entities/auth/auth.service';
import { LanguageService } from '../../services/language.service';
import { ExceptionBaseComponent } from '../base/exception-base.component';
import { TenantRouter } from '../../helpers/tenant-router';
import { ConnectivityService } from '../../services/connectivity.service';
import { Observable } from 'rxjs';
import { SupportLanguages } from '../../../entities/common/models/support-languages.enum';

@Component({
  selector: 'app-server-error',
  templateUrl: 'server-error.component.html',
  styleUrls: ['server-error.component.scss'],
  standalone: false,
})
export class ServerErrorComponent extends ExceptionBaseComponent {
  protected imageCode = '500';
  protected accent = 'coral' as const;
  public readonly isOnline$: Observable<boolean>;
  public offlineImageFailed = false;

  constructor(
    router: Router,
    authService: AuthService,
    languageService: LanguageService,
    tenantRouter: TenantRouter,
    tenantContextService: TenantContextService,
    private readonly connectivityService: ConnectivityService
  ) {
    super(
      router,
      authService,
      languageService,
      tenantRouter,
      tenantContextService
    );
    this.isOnline$ = this.connectivityService.isOnline$;
  }

  public retry(): void {
    window.location.reload();
  }

  public get offlineImgPath(): string {
    const language = this.languageService.getCurrentLanguage();
    const langSuffix = language === SupportLanguages.BG ? 'bg' : 'en';
    return `shared/assets/images/exceptions/offline_${langSuffix}.jpg`;
  }
}
