import { TenantRouter } from './../../helpers/tenant-router';
import { Router } from '@angular/router';
import { AuthService } from '../../../entities/auth/auth.service';
import { LanguageService } from '../../services/language.service';
import { SupportLanguages } from '../../../entities/common/models/support-languages.enum';
import { FULL_ROUTE, ROUTE } from '../../configs/route.config';
import { TenantContextService } from '../../services/tenant-context.service';

export abstract class ExceptionBaseComponent {
  protected abstract imageCode: string;

  constructor(
    protected readonly router: Router,
    protected readonly authService: AuthService,
    protected readonly languageService: LanguageService,
    private readonly tenantRouter: TenantRouter,
    private readonly tenantContextService: TenantContextService
  ) {}

  public redirectTo(): void {
    if (
      this.authService.getUser?.tenantId !== this.tenantContextService.tenantId
    ) {
      this.authService.logout().subscribe({
        next: () => {
          this.router.navigateByUrl(ROUTE.CHOOSE_CLUB);
        },
        error: () => {
          this.authService.logout(true).subscribe({
            next: () => {
              this.router.navigateByUrl(ROUTE.CHOOSE_CLUB);
            },
          });
        },
      });
      return;
    }

    if (this.authService.getUser) {
      this.tenantRouter.navigateTenant(FULL_ROUTE.GAMES.LIBRARY);
      return;
    }

    if (this.tenantContextService.hasTenant()) {
      this.tenantRouter.navigateTenant(ROUTE.LOGIN);
      return;
    }

    this.router.navigateByUrl(ROUTE.CHOOSE_CLUB);
  }

  public get imgPath(): string {
    const language = this.languageService.getCurrentLanguage();
    const langSuffix = language === SupportLanguages.BG ? 'bg' : 'en';
    return `shared/assets/images/exceptions/${this.imageCode}_${langSuffix}.jpg`;
  }
}
