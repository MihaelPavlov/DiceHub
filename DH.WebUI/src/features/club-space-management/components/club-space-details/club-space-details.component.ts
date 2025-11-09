import { BehaviorSubject, map } from 'rxjs';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Params, Router } from '@angular/router';
import { SpaceManagementService } from '../../../../entities/space-management/api/space-management.service';
import { IUserActiveSpaceTableResult } from '../../../../entities/space-management/models/user-active-space-table.model';
import { ISpaceTableParticipant } from '../../../../entities/space-management/models/table-participant.model';
import { AuthService } from '../../../../entities/auth/auth.service';
import { UserRole } from '../../../../entities/auth/enums/roles.enum';
import { FULL_ROUTE } from '../../../../shared/configs/route.config';
import { IMenuItem } from '../../../../shared/models/menu-item.model';
import { ISpaceTableById } from '../../../../entities/space-management/models/get-space-table-by-id.model';
import { NavigationService } from '../../../../shared/services/navigation-service';
import { TranslateService } from '@ngx-translate/core';
import { ToastType } from '../../../../shared/models/toast.model';
import { AppToastMessage } from '../../../../shared/components/toast/constants/app-toast-messages.constant';
import { ToastService } from '../../../../shared/services/toast.service';
import { DateHelper } from '../../../../shared/helpers/date-helper';
import { LanguageService } from '../../../../shared/services/language.service';
import { SupportLanguages } from '../../../../entities/common/models/support-languages.enum';

@Component({
  selector: 'app-club-space-details',
  templateUrl: 'club-space-details.component.html',
  styleUrl: 'club-space-details.component.scss',
})
export class ClubSpaceDetailsComponent implements OnInit {
  public userActiveTable!: IUserActiveSpaceTableResult;
  private allParticipants$ = new BehaviorSubject<ISpaceTableParticipant[]>([]);
  public readonly DATE_TIME_FORMAT: string = DateHelper.DATE_TIME_FORMAT;

  public spaceTableParticipantList$ = this.allParticipants$.asObservable();
  public tableId!: number;
  public menuItems: BehaviorSubject<IMenuItem[]> = new BehaviorSubject<
    IMenuItem[]
  >([]);

  public detailsSpaceTable!: ISpaceTableById;
  constructor(
    private readonly router: Router,
    private readonly activeRoute: ActivatedRoute,
    private readonly spaceManagementService: SpaceManagementService,
    private readonly authService: AuthService,
    private readonly navigationService: NavigationService,
    private readonly translateService: TranslateService,
    private readonly toastService: ToastService,
    private readonly languageService: LanguageService
  ) {}

  public get getCurrentLanguage(): SupportLanguages {
    return this.languageService.getCurrentLanguage();
  }

  public get isUserCreatorOfTable() {
    return this.detailsSpaceTable.createdBy === this.authService.getUser?.id;
  }

  public get isUserAdmin(): boolean {
    if (this.authService.getUser) {
      return this.authService.getUser.role !== UserRole.User ? true : false;
    }
    return false;
  }

  public handleMenuItemClick(key: string) {
    this.menuItemClickFunction(key);
  }

  public setParticipants(participants: ISpaceTableParticipant[]) {
    this.allParticipants$.next(participants);
  }

  public menuItemClickFunction(key: string): void {
    if (key === 'update') {
      this.navigationService.setPreviousUrl(this.router.url);
      this.router.navigateByUrl(
        FULL_ROUTE.SPACE_MANAGEMENT.UPDATE_BY_ID(this.tableId)
      );
    } else if (key === 'add-virtual-user') {
      this.addVirtualParticipant();
    } else if (key === 'close-room') {
      this.onClose();
    } else if (key === 'leave-room') {
      this.onLeave();
    }
  }

  public ngOnInit(): void {
    this.spaceManagementService.getUserActiveTable().subscribe({
      next: (result) => {
        this.userActiveTable = result;
      },
    });
    this.activeRoute.params.subscribe((params: Params) => {
      this.tableId = params['id'];

      this.getSpaceTableParticipantList();

      this.spaceManagementService.getTableById(this.tableId).subscribe({
        next: (result) => {
          this.detailsSpaceTable = result;
          this.menuItems.next([
            {
              key: 'update',
              label: this.translateService.instant(
                'space_management.details.menu_items.update'
              ),
              isVisible: this.isUserCreatorOfTable,
            },
            {
              key: 'add-virtual-user',
              label: this.translateService.instant(
                'space_management.details.menu_items.add_virtual_user'
              ),
              isVisible: this.isUserAdmin,
            },
            {
              key: 'close-room',
              label: this.translateService.instant(
                'space_management.details.menu_items.close_room'
              ),
              isVisible: this.isUserCreatorOfTable,
            },
            {
              key: 'leave-room',
              label: this.translateService.instant(
                'space_management.details.menu_items.leave_room'
              ),
              isVisible: this.userActiveTable
                ? this.userActiveTable.isPlayerParticipateInTable
                : false,
            },
          ]);
        },
      });
    });
  }

  public backToSpaceHome(): void {
    this.router.navigateByUrl(FULL_ROUTE.SPACE_MANAGEMENT.HOME);
  }

  public handleSearchExpression(searchExpression: string) {
    this.spaceTableParticipantList$ = this.allParticipants$.pipe(
      map((participants) =>
        searchExpression
          ? participants.filter((p) =>
              p.userName?.toLowerCase().includes(searchExpression.toLowerCase())
            )
          : participants
      )
    );
  }

  public onLeave(): void {
    this.spaceManagementService.leaveTable(this.tableId).subscribe({
      next: () => {
        this.router.navigateByUrl(FULL_ROUTE.SPACE_MANAGEMENT.HOME);
      },
    });
  }

  public onClose(): void {
    this.spaceManagementService.closeTable(this.tableId).subscribe({
      next: () => {
        this.router.navigateByUrl(FULL_ROUTE.SPACE_MANAGEMENT.HOME);
      },
    });
  }

  public addVirtualParticipant(): void {
    this.spaceManagementService.addVirtualParticipant(this.tableId).subscribe({
      next: () => {
        this.getSpaceTableParticipantList();
      },
      error: (error) => {
        const errorMessage = error.error.errors.MaxPeople[0];
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

  public onRemoveUserFromTable(
    userId: string,
    isVirtualParticipant: boolean,
    participantId: number
  ): void {
    if (isVirtualParticipant) {
      this.spaceManagementService
        .removeVirtualParticipant(this.tableId, participantId)
        .subscribe({
          next: () => {
            this.getSpaceTableParticipantList();
          },
        });
    } else {
      this.spaceManagementService
        .removeUserFromTable(this.tableId, userId)
        .subscribe({
          next: () => {
            this.getSpaceTableParticipantList();
          },
        });
    }
  }

  private getSpaceTableParticipantList(): void {
    this.spaceManagementService
      .getSpaceTableParticipantList(this.tableId)
      .subscribe({
        next: (result) => {
          this.allParticipants$.next(result ?? []);
        },
      });
  }
}
