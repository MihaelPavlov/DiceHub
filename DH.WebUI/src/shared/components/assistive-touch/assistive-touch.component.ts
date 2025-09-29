import { TenantUserSettingsService } from './../../../entities/common/api/tenant-user-settings.service';
import {
  Component,
  EventEmitter,
  HostListener,
  Input,
  OnDestroy,
  OnInit,
  Output,
} from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { NotificationsDialog } from './notifications/notifications.dialog';
import { BehaviorSubject, Subscription, timer } from 'rxjs';
import { AssistiveTouchSettings } from '../../../entities/common/models/assistive-touch-settings.model';
import { NotificationsService } from '../../../entities/common/api/notifications.service';

@Component({
  selector: 'app-assistive-touch',
  templateUrl: 'assistive-touch.component.html',
  styleUrl: 'assistive-touch.component.scss',
})
export class AssistiveTouchComponent implements OnInit, OnDestroy {
  public buttonOpacity: number = 1; // Button opacity (1 = fully visible)
  private inactivityTimer!: Subscription;
  @Input() areAnyActiveNotifications!: BehaviorSubject<boolean>;
  @Output() updateUserNotifications: EventEmitter<void> =
    new EventEmitter<void>();
  private assistiveTouchSettings: AssistiveTouchSettings | null = null;
  public positionY = this.assistiveTouchSettings
    ? Number(this.assistiveTouchSettings.positionY)
    : 135; // Initial Y position of the button
  public positionX = 0; // Initial X position of the button (left side)
  
  private isDragging = false;
  private dragOffsetX = 0; // To store the x offset from where the drag started
  private dragOffsetY = 0; // To store the y offset from where the drag started
  private dialogOpened = false;
  public isScreenFrozen = false;

  private readonly dragThreshold = 1; // Distance threshold to differentiate click vs. drag
  private initialX = 0; // Initial X position when drag starts
  private initialY = 0; // Initial Y position when drag starts
  private canOpenPanel = false;
  public subscriptionRefreshForAnyActiveNotifications!: any;

  public notificationsAllowed: boolean = false;
  constructor(
    private readonly dialog: MatDialog,
    private readonly tenantUserSettingsService: TenantUserSettingsService,
    private readonly notificationService: NotificationsService
  ) {
    this.notificationsAllowed = Notification.permission === 'granted';

    this.tenantUserSettingsService.getAssistiveTouchSettings().subscribe({
      next: (setting) => {
        if (setting) {
          this.assistiveTouchSettings = setting;
          this.initiatePosition();
        } else if (!setting) {
          this.tenantUserSettingsService.updateAssistiveTouchSettings({
            positionY: this.positionY.toString(),
            positionX: this.positionX.toString(),
          });
        }
      },
    });
    this.tenantUserSettingsService.assistiveTouchSettings$.subscribe({
      next: (setting) => {
        this.assistiveTouchSettings = setting;
      },
    });
    this.resetInactivityTimer();
  }

  public ngOnInit(): void {
    this.refreshForAnyActiveNotifications();
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
    this.isScreenFrozen = true;

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
  public stopDragging(event:  MouseEvent | TouchEvent): void {
    if (!this.isDragging) return;
    this.isScreenFrozen = false;

    this.isDragging = false;
  let currentX: number;
  let currentY: number;

  if (event instanceof MouseEvent) {
    currentX = event.clientX;
    currentY = event.clientY;
  } else if (event instanceof TouchEvent) {
    // touchend event's touches may be empty, use changedTouches instead
    const touch = event.changedTouches[0];
    currentX = touch.clientX;
    currentY = touch.clientY;
  } else {
    // fallback to initial positions if unknown event
    currentX = this.initialX;
    currentY = this.initialY;
  }

  const distanceMoved = Math.sqrt(
    Math.pow(currentX - this.initialX, 2) + Math.pow(currentY - this.initialY, 2)
  );

  console.log('distanceMoved > this.dragThreshold', distanceMoved, this.dragThreshold);

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
      event.preventDefault();

      this.resetInactivityTimer();
      // Check for single touch
      const touch = event.touches[0];
      this.updatePosition(touch.clientX, touch.clientY);
    }
  }

  @HostListener('window:touchstart', ['$event'])
public onTouchStart(event: TouchEvent) {
  if (this.isDragging) {
    event.preventDefault();
  }
}

@HostListener('window:touchend', ['$event'])
public onTouchEnd(event: TouchEvent) {
  if (this.isDragging) {
    event.preventDefault();
  }
}

  // Update button position with boundary checks
  private updatePosition(clientX: number, clientY: number): void {
    const buttonWidth = 38.4; // Those 32px are coming from the css padding of the button. This is the actual size of it.
    // check class .assistive_btn -> padding: 1rem
    const buttonHeight = 38.4; // Assuming button height is 50px
    const screenWidth = window.visualViewport
      ? window.visualViewport.width
      : window.innerWidth;
    const screenHeight = window.visualViewport
      ? window.visualViewport.height
      : window.innerHeight;
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
  }

  // Resets the inactivity timer
  private resetInactivityTimer(): void {
    this.inactivityTimer?.unsubscribe(); // Clear existing timer if any
    this.buttonOpacity = 1; // Make the button fully visible
    this.inactivityTimer = timer(3000).subscribe(() => {
      this.buttonOpacity = 0.5; // Reduce opacity after 3 seconds of inactivity
    });
  }

private snapToEdge(): void {
  const screenWidth = window.visualViewport
    ? window.visualViewport.width
    : window.innerWidth;
  const halfScreenWidth = screenWidth / 2;

  if (this.positionX < halfScreenWidth) {
    this.positionX = 0; // Snap to left
  } else {
    this.positionX = screenWidth - 38.4; // Snap to right (assuming button width is 32px)
  }

  this.tenantUserSettingsService.updateAssistiveTouchSettings({
    positionY: this.positionY.toString(),
    positionX: this.positionX.toString(), // store actual snapped X instead of isLeftAligned
  });
}

private initiatePosition(): void {
  const screenWidth = window.visualViewport
    ? window.visualViewport.width
    : window.innerWidth;
  const buttonHeight = 38.4;
  const screenHeight = window.visualViewport
    ? window.visualViewport.height
    : window.innerHeight;

  const bottomBoundary = screenHeight - buttonHeight - 76.8;
  const topBoundary = 76.8;

  this.positionY = Math.max(
    topBoundary,
    Math.min(Number(this.assistiveTouchSettings?.positionY), bottomBoundary)
  );

  // Use stored X position (fallback: right side if not set)
  this.positionX =
    this.assistiveTouchSettings?.positionX !== undefined
      ? Number(this.assistiveTouchSettings.positionX)
      : screenWidth - 38.4;
}
}
