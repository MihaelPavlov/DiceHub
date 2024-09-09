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
  public roomInfoMessages: IRoomInfoMessage[] = [
    {
      createdDate: new Date('2024-08-10'),
      message: 'Message 1',
    },
    {
      createdDate: new Date('2024-09-11'),
      message: 'Message 2',
    },
  ];
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
      this.roomService.checkUserParticipateInRoom(this.roomId),
    ]).subscribe({
      next: ([room, messages, isParticipate]) => {
        if (room && messages) {
          this.room = room;
          this.roomMessages = messages;
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
    console.log('scrolling to bottom', this.chatContainer);

    this.chatContainer.nativeElement.scrollTop =
      this.chatContainer.nativeElement.scrollHeight;
  }

  private startConnection(): void {
    const token = localStorage.getItem('jwt');
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl('https://localhost:7024/chatHub', {
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
          (sender, senderUsername, message, timestamp) => {
            console.log(
              `Received message from ${sender}: ${message} at ${timestamp}`
            );
            const groups = this.currentChatMessagesSubject$.value;

            let currentGroup2: GroupedChatMessage | null | undefined =
              groups.pop();

            let newGroup: GroupedChatMessage | null = null;
            if (currentGroup2 && currentGroup2.senderId === sender) {
              currentGroup2.messages.push({
                dateCreated: timestamp,
                text: message,
              });
              groups.push(currentGroup2);
            } else {
              if (currentGroup2) {
                groups.push(currentGroup2);
              }
              newGroup = {
                senderId: sender,
                senderUsername: senderUsername,
                messages: [{ dateCreated: timestamp, text: message }],
                dateCreated: timestamp,
                infoMessages: [],
              };
            }

            if (newGroup) {
              groups.push(newGroup);
            }

            this.currentChatMessagesSubject$.next(groups);
            this.shouldScrollToBottom = true;
            this.cdRef.detectChanges();
          }
        );
      })
      .catch((err) => console.log('Error while starting connection: ' + err));
  }

  public updateRoomMessages() {
    let currentGroup: GroupedChatMessage | null = null;
    const groups: GroupedChatMessage[] = [];
    for (const message of this.roomMessages) {
      if (currentGroup && currentGroup.senderId === message.senderId) {
        currentGroup.messages.push({
          dateCreated: message.createdDate,
          text: message.message,
        });
      } else {
        if (currentGroup) {
          groups.push(currentGroup);
        }
        currentGroup = {
          senderId: message.senderId,
          senderUsername: message.senderUsername,
          messages: [
            { dateCreated: message.createdDate, text: message.message },
          ],
          dateCreated: message.createdDate,
          infoMessages: [],
        };
      }
    }

    if (currentGroup) {
      groups.push(currentGroup);
    }
    const newGroups: GroupedChatMessage[] = [];

    for (let index = 0; index < groups.length; index++) {
      const currentGroup = groups[index];
      if (groups[index + 1]) {
        const nextGroup = groups[index + 1];
        //TODO: ALMOST working .
        // need to cover the cases if the date is before the compared groups and etc
        const smallestDateCreatedCurrentGroup = Math.min(
          ...currentGroup.messages.map((infoMessage) =>
            new Date(infoMessage.dateCreated).valueOf()
          )
        );

        const smallestDateCreatedNextGroup = Math.min(
          ...nextGroup.messages.map((infoMessage) =>
            new Date(infoMessage.dateCreated).valueOf()
          )
        );
        console.log(
          'smallestDateCreatedCurrentGroup',
          new Date(smallestDateCreatedCurrentGroup)
        );
        console.log(
          'smallestDateCreatedNextGroup',
          new Date(smallestDateCreatedNextGroup)
        );

        console.log(this.roomInfoMessages);
        const filteredInfoMessagesInMiddle = this.roomInfoMessages.filter(
          (x) => {
            console.log(x.createdDate);

            return (
              x.createdDate >= new Date(smallestDateCreatedCurrentGroup) &&
              x.createdDate <= new Date(smallestDateCreatedNextGroup)
            );
          }
        );

        const filteredInfoMessagesBefore = this.roomInfoMessages.filter((x) => {
          console.log(x.createdDate);

          return x.createdDate <= new Date(smallestDateCreatedCurrentGroup);
        });

        const filteredInfoMessagesAfter = this.roomInfoMessages.filter((x) => {
          console.log(x.createdDate);

          return x.createdDate >= new Date(smallestDateCreatedNextGroup);
        });
        console.log(filteredInfoMessagesBefore);
        console.log(filteredInfoMessagesInMiddle);
        console.log(filteredInfoMessagesAfter);

        for (let j = 0; j < filteredInfoMessagesBefore.length; j++) {
          const infoMessage = filteredInfoMessagesBefore[j];

          newGroups.push({
            senderId: '',
            senderUsername: '',
            messages: [],
            dateCreated: infoMessage.createdDate,
            infoMessages: [
              {
                text: infoMessage.message,
                dateCreated: infoMessage.createdDate,
              },
            ],
          });
        }

        for (let j = 0; j < filteredInfoMessagesInMiddle.length; j++) {
          const infoMessage = filteredInfoMessagesInMiddle[j];

          currentGroup.infoMessages.push({
            dateCreated: infoMessage.createdDate,
            text: infoMessage.message,
          });
        }

        for (let j = 0; j < filteredInfoMessagesAfter.length; j++) {
          const infoMessage = filteredInfoMessagesAfter[j];

          newGroups.push({
            senderId: '',
            senderUsername: '',
            messages: [],
            dateCreated: infoMessage.createdDate,
            infoMessages: [
              {
                text: infoMessage.message,
                dateCreated: infoMessage.createdDate,
              },
            ],
          });
        }
      }
    }

    const sorted = groups.concat(newGroups).sort((a, b) => {
      return new Date(a.dateCreated).valueOf() - new Date(b.dateCreated).valueOf();
    });
    this.currentChatMessagesSubject$.next(sorted);
    this.shouldScrollToBottom = true;
    this.cdRef.detectChanges();
  }
}
