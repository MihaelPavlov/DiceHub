import { NavigationService } from './../../../../shared/services/navigation-service';
import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { RoomsService } from '../../../../entities/rooms/api/rooms.service';
import { ActivatedRoute, Params } from '@angular/router';
import { MenuTabsService } from '../../../../shared/services/menu-tabs.service';
import { NAV_ITEM_LABELS } from '../../../../shared/models/nav-items-labels.const';
import { IRoomByIdResult } from '../../../../entities/rooms/models/room-by-id.model';
import { combineLatest } from 'rxjs';
import { AuthService } from '../../../../entities/auth/auth.service';
import { MeepleRoomMenuComponent } from '../meeple-room-menu/meeple-room-menu.component';
import { ImageEntityType } from '../../../../shared/pipe/entity-image.pipe';
import { FULL_ROUTE } from '../../../../shared/configs/route.config';
import { DateHelper } from '../../../../shared/helpers/date-helper';
import {
  ImagePreviewDialog,
  ImagePreviewData,
} from '../../../../shared/dialogs/image-preview/image-preview.dialog';
import { MatDialog } from '@angular/material/dialog';
import { TranslateService } from '@ngx-translate/core';
import { ToastService } from '../../../../shared/services/toast.service';
import { AppToastMessage } from '../../../../shared/components/toast/constants/app-toast-messages.constant';
import { ToastType } from '../../../../shared/models/toast.model';
import { LanguageService } from '../../../../shared/services/language.service';
import { SupportLanguages } from '../../../../entities/common/models/support-languages.enum';
import { TenantRouter } from '../../../../shared/helpers/tenant-router';

@Component({
  selector: 'app-meeple-room-details',
  templateUrl: 'meeple-room-details.component.html',
  styleUrl: 'meeple-room-details.component.scss',
  standalone: false,
})
export class MeepleRoomDetailsComponent implements OnInit, OnDestroy {
  @ViewChild(MeepleRoomMenuComponent) menu!: MeepleRoomMenuComponent;

  public room!: IRoomByIdResult;
  public isCurrentUserParticipateInRoom: boolean = false;
  public roomId!: number;
  public errorMessage: string | null = null;

  public readonly DATE_FORMAT: string = DateHelper.DATE_FORMAT;
  public readonly TIME_FORMAT: string = DateHelper.TIME_FORMAT;
  public readonly ImageEntityType = ImageEntityType;

  constructor(
    private readonly roomService: RoomsService,
    private readonly activeRoute: ActivatedRoute,
    private readonly authService: AuthService,
    private readonly menuTabsService: MenuTabsService,
    private readonly tenantRouter: TenantRouter,
    private readonly navigationService: NavigationService,
    private readonly dialog: MatDialog,
    private readonly translateService: TranslateService,
    private readonly toastService: ToastService,
    private readonly languageService: LanguageService
  ) {
    this.menuTabsService.setActive(NAV_ITEM_LABELS.MEEPLE);
  }

  public ngOnDestroy(): void {
    this.menuTabsService.resetData();
  }

  public get currentLanguage(): SupportLanguages {
    return this.languageService.getCurrentLanguage();
  }

  public ngOnInit(): void {
    this.activeRoute.params.subscribe((params: Params) => {
      this.roomId = params['id'];
      this.fetchData();
    });
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

  public backNavigateBtn(): void {
    this.tenantRouter.navigateTenant(FULL_ROUTE.MEEPLE_ROOM.FIND);
  }

  public navigateToGameDetails(id: number): void {
    this.navigationService.setPreviousUrl(
      FULL_ROUTE.MEEPLE_ROOM.DETAILS_BY_ID(this.roomId)
    );
    this.tenantRouter.navigateTenant(FULL_ROUTE.GAMES.DETAILS(id));
  }

  public navigateToChat(id: number): void {
    this.tenantRouter.navigateTenant(
      FULL_ROUTE.MEEPLE_ROOM.CHAT_ROOM_BY_ID(id)
    );
  }

  public onJoinRoom(): void {
    this.roomService.join(this.roomId).subscribe({
      next: () => this.fetchData(),
      error: (error) => {
        const errorMessage = error.error.errors['maxPeople'][0];
        if (errorMessage) {
          this.errorMessage = errorMessage;

          this.toastService.error({
            message: errorMessage,
            type: ToastType.Error,
          });
        } else
          this.toastService.error({
            message: this.translateService.instant(
              AppToastMessage.SomethingWrong
            ),
            type: ToastType.Error,
          });
      },
    });
  }

  public onLeaveCompleted(): void {
    this.fetchData();
  }

  public currentUserId(): string {
    return this.authService.getUser?.id || '';
  }

  private fetchData(): void {
    combineLatest([
      this.roomService.getById(this.roomId),
      this.roomService.checkUserParticipateInRoom(this.roomId),
    ]).subscribe({
      next: ([room, isParticipate]) => {
        this.room = room;

        this.isCurrentUserParticipateInRoom =
          room.createdBy === this.authService.getUser?.id || isParticipate;

        if (this.menu) {
          this.menu.room = this.room;
          this.menu.isCurrentUserParticipateInRoom =
            this.isCurrentUserParticipateInRoom;
          this.menu.updateMenuItems();
        }
      },
    });
  }
}
