import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ROUTE } from '../../../../shared/configs/route.config';
import { TranslateService } from '@ngx-translate/core';
import { GetClubInfoModel } from '../../../../entities/profile/models/get-club-info.interface';
import { TenantSettingsService } from '../../../../entities/common/api/tenant-settings.service';

@Component({
    selector: 'app-club-info',
    templateUrl: 'club-info.component.html',
    styleUrl: 'club-info.component.scss',
    standalone: false
})
export class ClubInfo implements OnInit {
  public clubInfo: GetClubInfoModel | null = null;

  constructor(
    private readonly router: Router,
    private readonly tenantSettingsService: TenantSettingsService,
    private readonly ts: TranslateService
  ) {}

  public ngOnInit(): void {
    this.tenantSettingsService.getClubInfo().subscribe({
      next: (clubInfo) => {
        console.log(clubInfo);
        
        this.clubInfo = clubInfo;
      },
      error:(err)=>{
        console.log(err);
        
      }
    });
  }

  public localizeDaysOff(daysOff: string[]): string {
    daysOff.map((x) => this.ts.instant(`week_days_names.${x}`));
    return daysOff.join(', ');
  }

  public backNavigateBtn() {
    this.router.navigateByUrl(ROUTE.PROFILE.CORE);
  }
}
