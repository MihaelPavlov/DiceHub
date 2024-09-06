import {
  AfterContentChecked,
  AfterViewChecked,
  AfterViewInit,
  ChangeDetectorRef,
  Component,
  ElementRef,
  Input,
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

export interface GroupedChatMessage {
  senderId: string;
  messages: string[];
  dateCreated: Date;
}

@Component({
  selector: 'app-room-chat',
  templateUrl: 'room-chat.component.html',
  styleUrl: 'room-chat.component.scss',
})
export class RoomChatComponent implements OnInit, OnDestroy, AfterViewChecked {
  @ViewChild('chat') private chatContainer!: ElementRef;

  public room!: IRoomByIdResult;
  public roomMessages: IRoomMessageResult[] = [];
  public message!: string;
  public roomId!: number;
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

  private fetchData(): void {
    combineLatest([
      this.roomService.getById(this.roomId),
      this.roomService.getMessageList(this.roomId),
    ]).subscribe({
      next: ([room, messages]) => {
        if (room && messages) {
          this.room = room;
          this.roomMessages = messages;

          this.updateRoomMessages()
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
      .configureLogging(signalR.LogLevel.Debug)
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

        // this.hubConnection
        //   .invoke('FetchPreviousMessages', this.roomId)
        //   .then((x) => this.updateRoomMessages(x))
        //   .catch((err) => console.error(err));

        // this.hubConnection.off('ReceiveMessage');

        this.hubConnection.on(
          'ReceiveMessage',
          (sender, message, timestamp) => {
            console.log(
              `Received message from ${sender}: ${message} at ${timestamp}`
            );
            const groups = this.currentChatMessagesSubject$.value;

            let currentGroup2: GroupedChatMessage | null | undefined =
              groups.pop();

            let newGroup: GroupedChatMessage | null = null;
            if (currentGroup2 && currentGroup2.senderId === sender) {
              currentGroup2.messages.push(message);
              groups.push(currentGroup2);
            } else {
              if (currentGroup2) {
                groups.push(currentGroup2);
              }
              newGroup = {
                senderId: sender,
                messages: [message],
                dateCreated: timestamp,
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
        currentGroup.messages.push(message.message);
      } else {
        if (currentGroup) {
          groups.push(currentGroup);
        }
        currentGroup = {
          senderId: message.senderId,
          messages: [message.message],
          dateCreated: message.createdDate,
        };
      }
    }

    if (currentGroup) {
      groups.push(currentGroup);
    }

    this.currentChatMessagesSubject$.next(groups);
    this.shouldScrollToBottom = true;
    this.cdRef.detectChanges();
  }
}
