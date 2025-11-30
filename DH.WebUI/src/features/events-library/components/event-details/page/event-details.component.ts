import { Component } from '@angular/core';
import { IEventByIdResult } from '../../../../../entities/events/models/event-by-id.mode';
import { Observable } from 'rxjs';
import { EventsService } from '../../../../../entities/events/api/events.service';
import { ActivatedRoute, Params, Router } from '@angular/router';
import { NAV_ITEM_LABELS } from '../../../../../shared/models/nav-items-labels.const';
import { MenuTabsService } from '../../../../../shared/services/menu-tabs.service';
import { ToastService } from '../../../../../shared/services/toast.service';
import { AppToastMessage } from '../../../../../shared/components/toast/constants/app-toast-messages.constant';
import { ToastType } from '../../../../../shared/models/toast.model';
import { DateHelper } from '../../../../../shared/helpers/date-helper';
import { FULL_ROUTE } from '../../../../../shared/configs/route.config';
import { TranslateService } from '@ngx-translate/core';
import { LanguageService } from '../../../../../shared/services/language.service';
import { SupportLanguages } from '../../../../../entities/common/models/support-languages.enum';
import {
  ImagePreviewDialog,
  ImagePreviewData,
} from '../../../../../shared/dialogs/image-preview/image-preview.dialog';
import { MatDialog } from '@angular/material/dialog';

@Component({
  selector: 'app-event-details',
  templateUrl: 'event-details.component.html',
  styleUrl: 'event-details.component.scss',
})
export class EventDetailsComponent {
  public event$!: Observable<IEventByIdResult>;
  public isUserParticipateInEvent!: boolean;
  public DATE_TIME_FORMAT: string = DateHelper.DATE_TIME_FORMAT;
  public readonly SupportLanguages = SupportLanguages;

  private eventId!: number;

  constructor(
    private readonly eventService: EventsService,
    private readonly activeRoute: ActivatedRoute,
    private readonly menuTabsService: MenuTabsService,
    private readonly toastService: ToastService,
    private readonly router: Router,
    private readonly translateService: TranslateService,
    private readonly languageService: LanguageService,
    private readonly dialog: MatDialog
  ) {
    this.menuTabsService.setActive(NAV_ITEM_LABELS.EVENTS);
  }

  public ngOnInit(): void {
    this.activeRoute.params.subscribe((params: Params) => {
      this.eventId = params['id'];
      this.fetchEvent();
    });
  }

  public get currentLanguage(): SupportLanguages {
    return this.languageService.getCurrentLanguage();
  }

  public ngOnDestroy(): void {
    this.menuTabsService.resetData();
  }

  public onParticipate(id: number) {
    this.eventService.participate(id).subscribe({
      next: (_) => {
        this.toastService.success({
          message: this.translateService.instant(
            AppToastMessage.ChangesApplied
          ),
          type: ToastType.Success,
        });

        this.router.navigateByUrl(FULL_ROUTE.EVENTS.HOME);
      },
      error: (error) => {
        const errorMessage = error.error.errors['maxPeople'][0];
        if (errorMessage)
          this.toastService.error({
            message: errorMessage,
            type: ToastType.Error,
          });
        else
          this.toastService.error({
            message: this.translateService.instant(
              AppToastMessage.SomethingWrong
            ),
            type: ToastType.Error,
          });
      },
    });
  }

  public onRemoveParticipant(id: number): void {
    this.eventService.removeParticipant(id).subscribe({
      next: (isSuccessfully) => {
        if (isSuccessfully) {
          this.toastService.success({
            message: this.translateService.instant(
              AppToastMessage.ChangesApplied
            ),
            type: ToastType.Success,
          });
          this.router.navigateByUrl(FULL_ROUTE.EVENTS.HOME);
        } else {
          this.toastService.error({
            message: this.translateService.instant(
              AppToastMessage.SomethingWrong
            ),
            type: ToastType.Error,
          });
        }
      },
      error: (error) => {
        this.toastService.error({
          message: this.translateService.instant(
            AppToastMessage.SomethingWrong
          ),
          type: ToastType.Error,
        });
      },
    });
  }

  public navigateBackToEventList(): void {
    this.router.navigateByUrl(FULL_ROUTE.EVENTS.HOME);
  }

  public getImage(event: IEventByIdResult): Observable<string> {
    return this.eventService.getImage(event.isCustomImage, event.imageId);
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

  private fetchEvent(): void {
    this.event$ = this.eventService.getById(this.eventId);

    this.eventService
      .checkUserParticipation(this.eventId)
      .subscribe(
        (isUserParticipateInEvent) =>
          (this.isUserParticipateInEvent =
            isUserParticipateInEvent !== null
              ? isUserParticipateInEvent
              : false)
      );
  }
}
