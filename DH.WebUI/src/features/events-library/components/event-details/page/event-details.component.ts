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
import { GameImagePipe } from '../../../../../shared/pipe/game-image.pipe';
import { EventImagePipe } from '../../../../../shared/pipe/event-image.pipe';

@Component({
  selector: 'app-event-details',
  templateUrl: 'event-details.component.html',
  styleUrl: 'event-details.component.scss',
})
export class EventDetailsComponent {
  public event$!: Observable<IEventByIdResult>;
  public isUserParticipateInEvent!: boolean;
  private eventId!: number;

  constructor(
    private readonly eventService: EventsService,
    private readonly activeRoute: ActivatedRoute,
    private readonly menuTabsService: MenuTabsService,
    private readonly toastService: ToastService,
    private readonly gameImagePipe: GameImagePipe,
    private readonly eventImagePipe: EventImagePipe,
    private readonly router: Router
  ) {
    this.menuTabsService.setActive(NAV_ITEM_LABELS.EVENTS);
  }

  public ngOnInit(): void {
    this.activeRoute.params.subscribe((params: Params) => {
      this.eventId = params['id'];
      this.fetchEvent();
    });
  }

  public ngOnDestroy(): void {
    this.menuTabsService.resetData;
  }

  public onParticipate(id: number) {
    this.eventService.participate(id).subscribe({
      next: (_) => {
        this.toastService.success({
          message: AppToastMessage.ChangesApplied,
          type: ToastType.Success,
        });

        this.router.navigateByUrl('/events/library');
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
            message: AppToastMessage.SomethingWrong,
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
            message: AppToastMessage.ChangesApplied,
            type: ToastType.Success,
          });
          this.router.navigateByUrl('/events/library');
        } else {
          this.toastService.error({
            message: AppToastMessage.SomethingWrong,
            type: ToastType.Error,
          });
        }
      },
      error: (error) => {
        this.toastService.error({
          message: AppToastMessage.SomethingWrong,
          type: ToastType.Error,
        });
      },
    });
  }

  public navigateBackToEventList(): void {
    this.router.navigateByUrl('/events/library');
  }

  public getImage(event: IEventByIdResult): string {
    if (event.isCustomImage) {
      return this.eventImagePipe.transform(event.imageId);
    }
    return this.gameImagePipe.transform(event.imageId);
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
