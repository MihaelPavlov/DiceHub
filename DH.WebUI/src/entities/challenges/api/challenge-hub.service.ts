import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { environment } from '../../../shared/environments/environment.development';
import { ROUTE } from '../../../shared/configs/route.config';
import { FrontEndLogService } from '../../../shared/services/frontend-log.service';
import { ToastService } from '../../../shared/services/toast.service';
import { TranslateService } from '@ngx-translate/core';
import { AppToastMessage } from '../../../shared/components/toast/constants/app-toast-messages.constant';
import { ToastType } from '../../../shared/models/toast.model';
import { ChallengeOverlayComponent } from '../../../shared/components/challenge-overlay/challenge-overlay.component';

@Injectable({ providedIn: 'root' })
export class ChallengeHubService {
  private hubConnection: signalR.HubConnection | null = null;

  constructor(
    private readonly toastService: ToastService,
    private readonly translateService: TranslateService,
    private readonly frontEndLogService: FrontEndLogService
  ) {}

  public async initChallengeHubConnection(
    userId: string,
    challengeOverlay: ChallengeOverlayComponent
  ): Promise<void> {
    await this.startConnection(userId);

    this.onChallengeUpdate((update) => {
      challengeOverlay.updateChallengeProgress(update.challengeGameName);
    });

    this.onChallengeCompleted((completed) => {
      challengeOverlay.completeChallenge(
        completed.challengeGameName,
        completed.rewardPoints
      );
    });

    this.onUniversalChallengeUpdate((update) => {
      challengeOverlay.updateUniversalChallengeProgress(
        update.challengeName_en,
        update.challengeName_bg
      );
    });

    this.onUniversalChallengeCompleted((completed) => {
      challengeOverlay.completeUniversalChallenge(
        completed.challengeName_en,
        completed.challengeName_bg,
        completed.rewardPoints
      );
    });

    this.onUniversalChallengeRestarted((completed) => {
      challengeOverlay.restartChallengeProgress(
        completed.challengeName_en,
        completed.challengeName_bg
      );
    });

    this.onRewardGranted((reward) => {
      challengeOverlay.rewardGranted(reward.name_bg, reward.name_en);
    });
  }

  public async startConnection(userId: string): Promise<void> {
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(
        `${environment.defaultAppUrl}/${ROUTE.CHALLENGE_HUB_CLIENT.CORE}?userId=${userId}`
      )
      .withAutomaticReconnect()
      .build();
    console.log('Starting SignalR connection to Challenge Hub');

    try {
      await this.hubConnection.start();
    } catch (err) {
      this.frontEndLogService.sendError(
        JSON.stringify(err),
        'challenge-hub.service.ts - startConnection()'
      );
      this.toastService.error({
        message: this.translateService.instant(AppToastMessage.SomethingWrong),
        type: ToastType.Error,
      });
    }
  }

  public onChallengeUpdate(callback: (data: any) => void): void {
    this.hubConnection!.on('challengeUpdated', callback);
  }

  public onChallengeCompleted(callback: (data: any) => void): void {
    this.hubConnection!.on('challengeCompleted', callback);
  }

  public onUniversalChallengeUpdate(callback: (data: any) => void): void {
    this.hubConnection!.on('universalChallengeUpdated', callback);
  }

  public onUniversalChallengeCompleted(callback: (data: any) => void): void {
    this.hubConnection!.on('universalChallengeCompleted', callback);
  }

  public onUniversalChallengeRestarted(callback: (data: any) => void): void {
    this.hubConnection!.on('universalChallengeRestarted', callback);
  }

  public onRewardGranted(callback: (data: any) => void): void {
    this.hubConnection!.on('rewardGranted', callback);
  }
}
