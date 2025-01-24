import {
  AfterViewChecked,
  ChangeDetectorRef,
  Component,
  ElementRef,
  OnDestroy,
  OnInit,
  ViewChild,
} from '@angular/core';
import { NAV_ITEM_LABELS } from '../../../../shared/models/nav-items-labels.const';
import { RoomsService } from '../../../../entities/rooms/api/rooms.service';
import { MenuTabsService } from '../../../../shared/services/menu-tabs.service';
import { AuthService } from '../../../../entities/auth/auth.service';
import { ActivatedRoute, Params, Router } from '@angular/router';
import * as signalR from '@microsoft/signalr';
import { BehaviorSubject, combineLatest, throwError } from 'rxjs';
import { IRoomByIdResult } from '../../../../entities/rooms/models/room-by-id.model';
import { ToastService } from '../../../../shared/services/toast.service';
import { AppToastMessage } from '../../../../shared/components/toast/constants/app-toast-messages.constant';
import { ToastType } from '../../../../shared/models/toast.model';
import { IRoomMessageResult } from '../../../../entities/rooms/models/room-message.model';
import { MeepleRoomMenuComponent } from '../meeple-room-menu/meeple-room-menu.component';
import { GroupedChatMessage } from './models/grouped-chat-messages.model';
import { IRoomInfoMessageResult } from '../../../../entities/rooms/models/room-info-message.model';
import { environment } from '../../../../shared/environments/environment.development';

export interface IRoomInfoMessage {
  createdDate: Date;
  message: string;
}
@Component({
  selector: 'app-room-chat',
  templateUrl: 'room-chat.component.html',
  styleUrl: 'room-chat.component.scss',
})
export class RoomChatComponent implements OnInit, OnDestroy, AfterViewChecked {
  @ViewChild('chat') private chatContainer!: ElementRef;
  @ViewChild(MeepleRoomMenuComponent) menu!: MeepleRoomMenuComponent;
  public roomInfoMessages: IRoomInfoMessageResult[] = [];
  public room!: IRoomByIdResult;
  public roomMessages: IRoomMessageResult[] = [];
  public message!: string;
  public roomId!: number;
  public isCurrentUserParticipateInRoom: boolean = false;
  private shouldScrollToBottom = false;
  public currentChatMessagesSubject$: BehaviorSubject<GroupedChatMessage[]> =
    new BehaviorSubject<GroupedChatMessage[]>([]);
  private hubConnection!: signalR.HubConnection;

  constructor(
    private readonly roomService: RoomsService,
    private readonly menuTabsService: MenuTabsService,
    private readonly authService: AuthService,
    private readonly cdRef: ChangeDetectorRef,
    private readonly router: Router,
    private readonly activeRoute: ActivatedRoute,
    private readonly toastService: ToastService
  ) {
    this.menuTabsService.setActive(NAV_ITEM_LABELS.MEEPLE);
  }
  public ngAfterViewChecked(): void {
    if (this.shouldScrollToBottom) {
      this.scrollToBottom();
      this.shouldScrollToBottom = false;
    }
  }
  public ngOnInit(): void {
    this.activeRoute.params.subscribe((params: Params) => {
      this.roomId = +params['id'];
      this.fetchData();
      this.startConnection();
    });
  }

  public ngOnDestroy(): void {
    this.menuTabsService.resetData();
    this.hubConnection.stop();
  }

  public addMessage() {
    if (!this.message) return;

    this.hubConnection
      .invoke('SendMessageToGroup', this.roomId, this.message)
      .then(() => (this.message = ''))
      .catch((err) => console.error(err));
  }

  public getActiveUserIdFromChat(): string {
    return this.authService.getUser ? this.authService.getUser.id : '';
  }

  public backNavigateBtn(): void {
    this.router.navigateByUrl(`meeples/${this.roomId}/details`);
  }

  public onLeaveCompleted(): void {
    this.backNavigateBtn();
  }

  private fetchData(): void {
    combineLatest([
      this.roomService.getById(this.roomId),
      this.roomService.getMessageList(this.roomId),
      this.roomService.getInfoMessageList(this.roomId),
      this.roomService.checkUserParticipateInRoom(this.roomId),
    ]).subscribe({
      next: ([room, messages, infoMessages, isParticipate]) => {
        if (room && messages) {
          this.room = room;
          this.roomMessages = messages;

          this.roomInfoMessages = infoMessages.map((x) => ({
            ...x,
            createdDate: new Date(x.createdDate),
          }));

          this.isCurrentUserParticipateInRoom =
            room.createdBy === this.authService.getUser?.id || isParticipate;

          this.updateRoomMessages();

          if (this.menu) {
            this.menu.room = this.room;
            this.menu.isCurrentUserParticipateInRoom =
              this.isCurrentUserParticipateInRoom;
            this.menu.updateMenuItems();
          }
        }
      },
      error: (error) => {
        this.toastService.error({
          message: AppToastMessage.SomethingWrong,
          type: ToastType.Error,
        });
        throwError(() => error);
      },
    });
  }

  private scrollToBottom(): void {
    this.chatContainer.nativeElement.scrollTo({
      top: this.chatContainer.nativeElement.scrollHeight,
      behavior: 'smooth',
    });
  }

  private startConnection(): void {
    const token = localStorage.getItem('jwt');
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(`${environment.defaultAppUrl}/${ROUTE.CHAT_HUB.CORE}`, {
        accessTokenFactory: (): any => token,
      })
      .build();

    this.hubConnection
      .start()
      .then(() => {
        this.hubConnection
          .invoke('ConnectToGroup', this.roomId)
          .catch((err) => console.error(err));

        this.hubConnection.on(
          'ReceiveMessage',
          (sender, senderUsername, message, createdDate) => {
            console.log(
              `Received message from ${sender}: ${message} at ${createdDate}`
            );
            const groups = this.currentChatMessagesSubject$.value;

            const currentGroup = {
              senderId: sender,
              senderUsername: senderUsername,
              messages: [{ createdDate: createdDate, text: message }],
              createdDate: createdDate,
              infoMessages: [],
            };

            console.log(currentGroup);

            groups.push(currentGroup);
            const sorted = groups.sort((a, b) => {
              return (
                new Date(a.createdDate).valueOf() -
                new Date(b.createdDate).valueOf()
              );
            });

            this.currentChatMessagesSubject$.next(sorted);
            this.shouldScrollToBottom = true;
            this.cdRef.detectChanges();
          }
        );
      })
      .catch((err) => console.log('Error while starting connection: ' + err));
  }

  public getPreviousSenderUsername(currentIndex: number): string {
    const groups = this.currentChatMessagesSubject$.value;
    if (groups[currentIndex - 1]) {
      return groups[currentIndex - 1].senderUsername;
    }

    return '';
  }

  public updateRoomMessages() {
    const groups = this.createGroupsFromRoomMessages();
    let newGroups = this.processGroupInfoMessages(groups);

    if (groups.length === 0) {
      newGroups = this.roomInfoMessages.map((infoMessage) => ({
        senderId: '',
        senderUsername: '',
        messages: [],
        createdDate: infoMessage.createdDate,
        infoMessages: [
          {
            text: infoMessage.message,
            createdDate: infoMessage.createdDate,
          },
        ],
      }));
    }

    const sorted = groups.concat(newGroups).sort((a, b) => {
      return (
        new Date(a.createdDate).valueOf() - new Date(b.createdDate).valueOf()
      );
    });

    this.currentChatMessagesSubject$.next(sorted);
    this.shouldScrollToBottom = true;
    this.cdRef.detectChanges();
  }

  // Step 1: Group room messages
  private createGroupsFromRoomMessages(): GroupedChatMessage[] {
    const groups: GroupedChatMessage[] = [];
    for (const message of this.roomMessages) {
      const currentGroup: GroupedChatMessage = {
        senderId: message.senderId,
        senderUsername: message.senderUsername,
        messages: [{ createdDate: message.createdDate, text: message.message }],
        createdDate: message.createdDate,
        infoMessages: [],
      };
      groups.push(currentGroup);
    }
    console.log('createGroupsFromRoomMessages -> ', groups);

    return groups;
  }

  // Step 2: Process info messages and place them before, after, or in between groups
  private processGroupInfoMessages(
    groups: GroupedChatMessage[]
  ): GroupedChatMessage[] {
    const newGroups: GroupedChatMessage[] = [];

    for (let index = 0; index < groups.length; index++) {
      const currentGroup = groups[index];
      const nextGroup = groups[index + 1];

      const smallestDateCreatedCurrentGroup =
        this.getSmallestDateCreated(currentGroup);
      const smallestDateCreatedNextGroup = nextGroup
        ? this.getSmallestDateCreated(nextGroup)
        : Number.MAX_SAFE_INTEGER;

      const filteredInfoMessagesBefore = this.getFilteredInfoMessagesBefore(
        smallestDateCreatedCurrentGroup
      );
      const filteredInfoMessagesInMiddle = this.getFilteredInfoMessagesInMiddle(
        smallestDateCreatedCurrentGroup,
        smallestDateCreatedNextGroup
      );
      const filteredInfoMessagesAfter = this.getFilteredInfoMessagesAfter(
        smallestDateCreatedNextGroup
      );

      this.addInfoMessagesToGroup(currentGroup, filteredInfoMessagesInMiddle);

      this.addFilteredInfoMessagesToNewGroups(
        filteredInfoMessagesBefore,
        newGroups
      );
      this.addFilteredInfoMessagesToNewGroups(
        filteredInfoMessagesAfter,
        newGroups
      );

      this.removeProcessedInfoMessages(
        filteredInfoMessagesBefore,
        filteredInfoMessagesInMiddle,
        filteredInfoMessagesAfter
      );
    }

    return newGroups;
  }

  private getSmallestDateCreated(group: GroupedChatMessage): number {
    return Math.min(
      ...group.messages.map((infoMessage) =>
        new Date(infoMessage.createdDate).valueOf()
      )
    );
  }

  private getFilteredInfoMessagesBefore(
    date: number
  ): IRoomInfoMessageResult[] {
    return this.roomInfoMessages.filter((x) => x.createdDate <= new Date(date));
  }

  private getFilteredInfoMessagesInMiddle(
    startDate: number,
    endDate: number
  ): IRoomInfoMessageResult[] {
    return this.roomInfoMessages.filter(
      (x) =>
        x.createdDate >= new Date(startDate) &&
        x.createdDate <= new Date(endDate)
    );
  }

  private getFilteredInfoMessagesAfter(date: number): IRoomInfoMessageResult[] {
    return this.roomInfoMessages.filter((x) => x.createdDate >= new Date(date));
  }

  private addInfoMessagesToGroup(
    group: GroupedChatMessage,
    infoMessages: IRoomInfoMessageResult[]
  ) {
    for (const infoMessage of infoMessages) {
      group.infoMessages.push({
        createdDate: infoMessage.createdDate,
        text: infoMessage.message,
      });
    }
  }

  private addFilteredInfoMessagesToNewGroups(
    infoMessages: IRoomInfoMessageResult[],
    newGroups: GroupedChatMessage[]
  ) {
    for (const infoMessage of infoMessages) {
      newGroups.push({
        senderId: '',
        senderUsername: '',
        messages: [],
        createdDate: infoMessage.createdDate,
        infoMessages: [
          {
            text: infoMessage.message,
            createdDate: infoMessage.createdDate,
          },
        ],
      });
    }
  }

  private removeProcessedInfoMessages(
    ...infoMessagesGroups: IRoomInfoMessageResult[][]
  ) {
    const allProcessedMessages = infoMessagesGroups.flat();
    this.roomInfoMessages = this.roomInfoMessages.filter(
      (x) => !allProcessedMessages.includes(x)
    );
  }
}
