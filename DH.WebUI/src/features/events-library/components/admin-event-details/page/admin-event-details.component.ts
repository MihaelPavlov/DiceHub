import { Component, OnDestroy, OnInit } from '@angular/core';
import { IEventByIdResult } from '../../../../../entities/events/models/event-by-id.mode';
import { Observable } from 'rxjs';
import { EventsService } from '../../../../../entities/events/api/events.service';
import { ActivatedRoute, Params, Router } from '@angular/router';
import { NAV_ITEM_LABELS } from '../../../../../shared/models/nav-items-labels.const';
import { MenuTabsService } from '../../../../../shared/services/menu-tabs.service';
import { FULL_ROUTE } from '../../../../../shared/configs/route.config';
import { DateHelper } from '../../../../../shared/helpers/date-helper';
import { LanguageService } from '../../../../../shared/services/language.service';
import { SupportLanguages } from '../../../../../entities/common/models/support-languages.enum';
import {
  ImagePreviewData,
  ImagePreviewDialog,
} from '../../../../../shared/dialogs/image-preview/image-preview.dialog';
import { TranslateService } from '@ngx-translate/core';
import { MatDialog } from '@angular/material/dialog';

@Component({
    selector: 'app-admin-event-details',
    templateUrl: 'admin-event-details.component.html',
    styleUrl: 'admin-event-details.component.scss',
    standalone: false
})
export class AdminEventDetailsComponent implements OnInit, OnDestroy {
  public event$!: Observable<IEventByIdResult>;

  public readonly DATE_TIME_FORMAT: string = DateHelper.DATE_TIME_FORMAT;
  public readonly SupportLanguages = SupportLanguages;
  private eventId!: number;

  constructor(
    private readonly eventService: EventsService,
    private readonly activeRoute: ActivatedRoute,
    private readonly menuTabsService: MenuTabsService,
    private readonly router: Router,
    private readonly languageService: LanguageService,
    private readonly translateService: TranslateService,
    private readonly dialog: MatDialog
  ) {
    this.menuTabsService.setActive(NAV_ITEM_LABELS.EVENTS);
  }

  public ngOnDestroy(): void {
    this.menuTabsService.resetData();
  }

  public get currentLanguage(): SupportLanguages {
    return this.languageService.getCurrentLanguage();
  }

  public ngOnInit(): void {
    this.activeRoute.params.subscribe((params: Params) => {
      this.eventId = params['id'];

      this.fetchEvent();
    });
  }

  public navigateBackToEventList(): void {
    this.router.navigateByUrl(FULL_ROUTE.EVENTS.HOME);
  }

  public navigateToUpdate(id: number): void {
    this.router.navigateByUrl(FULL_ROUTE.EVENTS.ADMIN.UPDATE_BY_ID(id));
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
  }
}
