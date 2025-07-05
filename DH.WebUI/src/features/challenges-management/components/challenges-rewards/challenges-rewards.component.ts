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

@Component({
  selector: 'app-challenges-rewards',
  templateUrl: 'challenges-rewards.component.html',
  styleUrl: 'challenges-rewards.component.scss',
})
export class ChallengesRewardsComponent implements OnInit {
  public userRewards$!: Observable<IUserReward[]>;

  public readonly UserRewardStatus = UserRewardStatus;
  public readonly ImageEntityType = ImageEntityType;

  constructor(
    private readonly rewardsService: RewardsService,
    private readonly authService: AuthService,
    private readonly dialog: MatDialog,
    private readonly router: Router
  ) {}

  public ngOnInit(): void {
    this.userRewards$ = this.rewardsService.getUserRewardList();
  }

  public openImagePreview(imageUrl: string) {
    this.dialog.open<ImagePreviewDialog, ImagePreviewData>(ImagePreviewDialog, {
      data: {
        imageUrl,
        title: 'Image',
      },
      width: '17rem',
    });
  }

  public navigateToChallenges(): void {
    this.router.navigateByUrl('challenges/home');
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
