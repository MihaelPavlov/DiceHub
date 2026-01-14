import { UserRewardStatus } from './../../../../entities/rewards/models/user-reward.model';
import { Component, OnInit } from '@angular/core';
import { RewardsService } from '../../../../entities/rewards/api/rewards.service';
import { Observable } from 'rxjs';
import { IUserReward } from '../../../../entities/rewards/models/user-reward.model';
import { UserRewardQrCodeDialog } from '../../dialogs/user-reward-qr-code-dialog/user-reward-qr-code.component';
import { MatDialog } from '@angular/material/dialog';
import { AuthService } from '../../../../entities/auth/auth.service';
import { QrCodeType } from '../../../../entities/qr-code-scanner/enums/qr-code-type.enum';
import { ImageEntityType } from '../../../../shared/pipe/entity-image.pipe';
import { Router } from '@angular/router';
import {
  ImagePreviewDialog,
  ImagePreviewData,
} from '../../../../shared/dialogs/image-preview/image-preview.dialog';
import { FULL_ROUTE } from '../../../../shared/configs/route.config';
import { TranslateService } from '@ngx-translate/core';
import { LanguageService } from '../../../../shared/services/language.service';
import { SupportLanguages } from '../../../../entities/common/models/support-languages.enum';
import { TenantRouter } from '../../../../shared/helpers/tenant-router';

@Component({
  selector: 'app-challenges-rewards',
  templateUrl: 'challenges-rewards.component.html',
  styleUrl: 'challenges-rewards.component.scss',
  standalone: false,
})
export class ChallengesRewardsComponent implements OnInit {
  public userRewards$!: Observable<IUserReward[]>;

  public readonly UserRewardStatus = UserRewardStatus;
  public readonly ImageEntityType = ImageEntityType;
  public readonly SupportLanguages = SupportLanguages;

  constructor(
    private readonly rewardsService: RewardsService,
    private readonly authService: AuthService,
    private readonly dialog: MatDialog,
    private readonly tenantRouter: TenantRouter,
    private readonly translateService: TranslateService,
    private readonly languageService: LanguageService
  ) {}

  public ngOnInit(): void {
    this.userRewards$ = this.rewardsService.getUserRewardList();
  }

  public get currentLanguage(): SupportLanguages {
    return this.languageService.getCurrentLanguage();
  }

  public openImagePreview(imageUrl: string) {
    this.dialog.open<ImagePreviewDialog, ImagePreviewData>(ImagePreviewDialog, {
      data: {
        imageUrl,
        title: this.translateService.instant('image'),
      },
      width: '17rem',
    });
  }

  public navigateToChallenges(): void {
    this.tenantRouter.navigateTenant(FULL_ROUTE.CHALLENGES.CHALLENGES_HOME);
  }

  public openDialog(id: number): void {
    this.dialog.open(UserRewardQrCodeDialog, {
      width: '17rem',
      data: {
        Id: id,
        Name: 'UserReward',
        Type: QrCodeType.Reward,
        AdditionalData: {
          userId: this.authService.getUser?.id,
        },
      },
    });
  }
}
