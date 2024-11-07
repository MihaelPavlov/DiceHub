import { TenantUserSettingsService } from './../../../entities/common/api/tenant-user-settings.service';
import {
  ChangeDetectorRef,
  Component,
  EventEmitter,
  HostListener,
  Input,
  NgZone,
  OnDestroy,
  OnInit,
  Output,
} from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { NotificationsDialog } from './notifications/notifications.dialog';
import { BehaviorSubject, Observable, Subscription, timer } from 'rxjs';
import { AssistiveTouchSettings } from '../../../entities/common/models/assistive-touch-settings.model';
import { NotificationsService } from '../../../entities/common/api/notifications.service';
import { Messaging } from '@angular/fire/messaging';

@Component({
  selector: 'app-assistive-touch',
  templateUrl: 'assistive-touch.component.html',
  styleUrl: 'assistive-touch.component.scss',
})
export class AssistiveTouchComponent implements OnInit, OnDestroy {
  public buttonOpacity = 1; // Button opacity (1 = fully visible)
  private inactivityTimer!: Subscription;
  @Input() areAnyActiveNotifications!: BehaviorSubject<boolean>;
  @Output() updateUserNotifications: EventEmitter<void> =
    new EventEmitter<void>();
  public areAnyActiveNotifications2!: Observable<boolean>;
  private assistiveTouchSettings: AssistiveTouchSettings | null = null;
  public positionY = this.assistiveTouchSettings
    ? Number(this.assistiveTouchSettings.positionY)
    : 256; // Initial Y position of the button
  public positionX = 0; // Initial X position of the button (left side)
  public isLeftAligned = this.assistiveTouchSettings
    ? this.assistiveTouchSettings.isLeftAligned
    : true;

  private isDragging = false;
  private dragOffsetX = 0; // To store the x offset from where the drag started
  private dragOffsetY = 0; // To store the y offset from where the drag started
  private dialogOpened = false;

  private readonly dragThreshold = 1; // Distance threshold to differentiate click vs. drag
  private initialX = 0; // Initial X position when drag starts
  private initialY = 0; // Initial Y position when drag starts
  private canOpenPanel = false;
  public subscriptionRefreshForAnyActiveNotifications!: any;
  constructor(
    private readonly dialog: MatDialog,
    private readonly tenantUserSettingsService: TenantUserSettingsService,
    private readonly notificationService: NotificationsService,
  ) {
    this.tenantUserSettingsService.getAssistiveTouchSettings().subscribe({
      next: (setting) => {
        if (setting) {
          this.assistiveTouchSettings = setting;
          this.initiatePosition();
        } else if (!setting) {
          this.tenantUserSettingsService.updateAssistiveTouchSettings({
            positionY: this.positionY.toString(),
            isLeftAligned: this.isLeftAligned,
          });
        }
      },
    });
    this.tenantUserSettingsService.assistiveTouchSettings$.subscribe({
      next: (setting) => {
        console.log('from assistive touch component ->', setting);

        this.assistiveTouchSettings = setting;
      },
    });
    this.resetInactivityTimer();
  }

  public ngOnInit(): void {
    this.subscriptionRefreshForAnyActiveNotifications = setInterval(
      () => this.refreshForAnyActiveNotifications(),
      10000
    );
  }
  public refreshForAnyActiveNotifications(): void {
    this.notificationService.areAnyActiveNotifications().subscribe({
      next: (res) => {
        this.areAnyActiveNotifications.next(res);
      },
    });
  }
  ngOnDestroy(): void {
    this.inactivityTimer?.unsubscribe(); // Clean up timer on component destroy
    if (this.subscriptionRefreshForAnyActiveNotifications)
      clearInterval(this.subscriptionRefreshForAnyActiveNotifications);
  }

  public openNotifications(): void {
    if (!this.canOpenPanel) return;

    if (this.dialogOpened) return; // Prevent opening if already open
    this.resetInactivityTimer();

    this.dialogOpened = true;
    const dialogRef = this.dialog.open(NotificationsDialog);

    dialogRef.componentInstance.notificationsUpdated.subscribe(() => {
      this.updateUserNotifications.emit();
    });

    dialogRef.afterClosed().subscribe(() => {
      this.dialogOpened = false;
      this.resetInactivityTimer();
    });
  }

  // Start dragging on mouse or touch start
  public startDragging(event: MouseEvent | TouchEvent): void {
    this.canOpenPanel = false;
    this.isDragging = true;
    // Calculate the offsets
    // Set initial positions for threshold comparison
    if (event instanceof MouseEvent) {
      this.initialX = event.clientX;
      this.initialY = event.clientY;
      this.dragOffsetX = event.clientX - this.positionX;
      this.dragOffsetY = event.clientY - this.positionY;
    } else if (event instanceof TouchEvent) {
      this.initialX = event.touches[0].clientX;
      this.initialY = event.touches[0].clientY;
      this.dragOffsetX = event.touches[0].clientX - this.positionX;
      this.dragOffsetY = event.touches[0].clientY - this.positionY;
    }
  }

  // Stop dragging on mouse or touch end
  public stopDragging(event: any): void {
    if (!this.isDragging) return;

    this.isDragging = false;
    const distanceMoved = Math.sqrt(
      Math.pow(event.clientX - this.initialX, 2) +
        Math.pow(event.clientY - this.initialY, 2)
    );

    // Only snap to edge if drag threshold is met
    if (distanceMoved > this.dragThreshold) {
      this.snapToEdge();
      timer(400).subscribe(() => {
        this.canOpenPanel = true;
      });
    } else {
      this.canOpenPanel = true;
      this.openNotifications(); // Treat as a click if below threshold
    }

    this.resetInactivityTimer();
  }

  // Track mouse move events
  @HostListener('window:mousemove', ['$event'])
  public onMouseMove(event: MouseEvent) {
    if (this.isDragging) {
      this.resetInactivityTimer();
      this.updatePosition(event.clientX, event.clientY);
    }
  }

  @HostListener('window:touchmove', ['$event'])
  public onTouchMove(event: TouchEvent) {
    if (this.isDragging && event.touches.length === 1) {
      this.resetInactivityTimer();
      // Check for single touch
      const touch = event.touches[0];
      this.updatePosition(touch.clientX, touch.clientY);
    }
  }
  // Update button position with boundary checks
  private updatePosition(clientX: number, clientY: number): void {
    const buttonWidth = 38.4; // Those 32px are coming from the css padding of the button. This is the actual size of it.
    // check class .assistive_btn -> padding: 1rem
    const buttonHeight = 38.4; // Assuming button height is 50px
    const screenWidth = window.innerWidth;
    const screenHeight = window.innerHeight;
    const bottomBoundary = screenHeight - buttonHeight - 76.8; // e.g., 60px from the bottom
    const topBoundary = 76.8; // e.g., 60px from the top

    // Update positions based on offsets
    this.positionX = clientX - this.dragOffsetX;
    this.positionY = clientY - this.dragOffsetY;

    // Ensure the button does not go outside the boundaries
    this.positionX = Math.max(
      0,
      Math.min(this.positionX, screenWidth - buttonWidth)
    );

    this.positionY = Math.max(
      topBoundary,
      Math.min(this.positionY, bottomBoundary)
    );
    console.log(this.positionX, this.positionY);
  }

  // Resets the inactivity timer
  private resetInactivityTimer(): void {
    this.inactivityTimer?.unsubscribe(); // Clear existing timer if any
    this.buttonOpacity = 1; // Make the button fully visible
    this.inactivityTimer = timer(3000).subscribe(() => {
      this.buttonOpacity = 0.5; // Reduce opacity after 3 seconds of inactivity
    });
  }

  // Snap to the nearest edge
  private snapToEdge(): void {
    const screenWidth = window.innerWidth;
    const halfScreenWidth = screenWidth / 2;

    if (this.positionX < halfScreenWidth) {
      this.positionX = 0; // Snap to left

      this.isLeftAligned = true;
      this.tenantUserSettingsService.updateAssistiveTouchSettings({
        positionY: this.positionY.toString(),
        isLeftAligned: true,
      });
    } else {
      this.positionX = screenWidth - 38.4; // Snap to right (assuming button width is 32px)

      this.isLeftAligned = false;

      this.tenantUserSettingsService.updateAssistiveTouchSettings({
        positionY: this.positionY.toString(),
        isLeftAligned: false,
      });
    }
  }

  private initiatePosition(): void {
    const screenWidth = window.innerWidth;
    const buttonHeight = 38.4; // Assuming button height is 50px
    const screenHeight = window.innerHeight;
    const bottomBoundary = screenHeight - buttonHeight - 76.8; // e.g., 60px from the bottom
    const topBoundary = 76.8; // e.g., 60px from the top
    if (this.assistiveTouchSettings)
      this.isLeftAligned = this.assistiveTouchSettings.isLeftAligned;

    this.positionY = Math.max(
      topBoundary,
      Math.min(Number(this.assistiveTouchSettings?.positionY), bottomBoundary)
    );
    if (this.assistiveTouchSettings?.isLeftAligned) {
      this.positionX = 0; // Snap to left
    } else {
      this.positionX = screenWidth - 38.4; // Snap to right (assuming button width is 32px)
    }
  }
}
