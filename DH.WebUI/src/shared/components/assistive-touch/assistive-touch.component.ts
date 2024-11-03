import { Component, HostListener } from '@angular/core';

@Component({
  selector: 'app-assistive-touch',
  templateUrl: 'assistive-touch.component.html',
  styleUrl: 'assistive-touch.component.scss',
})
export class AssistiveTouchComponent {
  public button1Left: number = 80;
  public button1Top: number = 0;
  public button1Right: number = 0;

  public button2Left: number = 20;
  public button2Top: number = 80;
  public button2Right: number = 0;

  public button3Left: number = 20;
  public button3Bottom: number = 80;
  public button3Right: number = 0;

  public positionY = 256; // Initial Y position of the button
  //   public positionX = 0; // Initial X position of the button (left side)
  public positionX = window.innerWidth - 32; // Initial X position of the button (right side)

  private isDragging = false;
  private dragOffsetX = 0; // To store the x offset from where the drag started
  private dragOffsetY = 0; // To store the y offset from where the drag started

  // Start dragging on mouse or touch start
  public startDragging(event: MouseEvent | TouchEvent): void {
    this.isDragging = true;

    // Calculate the offsets
    if (event instanceof MouseEvent) {
      this.dragOffsetX = event.clientX - this.positionX;
      this.dragOffsetY = event.clientY - this.positionY;
    } else if (event instanceof TouchEvent) {
      this.dragOffsetX = event.touches[0].clientX - this.positionX;
      this.dragOffsetY = event.touches[0].clientY - this.positionY;
    }
  }

  // Stop dragging on mouse or touch end
  public stopDragging(): void {
    this.isDragging = false;
    this.snapToEdge();
  }

  // Track mouse move events
  @HostListener('window:mousemove', ['$event'])
  public onMouseMove(event: MouseEvent) {
    if (this.isDragging) {
      this.updatePosition(event.clientX, event.clientY);
    }
  }

  @HostListener('window:touchmove', ['$event'])
  public onTouchMove(event: TouchEvent) {
    if (this.isDragging && event.touches.length === 1) {
      // Check for single touch
      const touch = event.touches[0];
      this.updatePosition(touch.clientX, touch.clientY);
    }
  }
  // Update button position with boundary checks
  private updatePosition(clientX: number, clientY: number): void {
    const buttonWidth = 32; // Those 32px are coming from the css padding of the button. This is the actual size of it.
    // check class .assistive_btn -> padding: 1rem
    const buttonHeight = 32; // Assuming button height is 50px
    const screenWidth = window.innerWidth;
    const screenHeight = window.innerHeight;
    const bottomBoundary = screenHeight - buttonHeight - 64; // e.g., 60px from the bottom
    const topBoundary = 64; // e.g., 60px from the top

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

  // Snap to the nearest edge
  private snapToEdge(): void {
    const screenWidth = window.innerWidth;
    const halfScreenWidth = screenWidth / 2;

    if (this.positionX < halfScreenWidth) {
      this.positionX = 0; // Snap to left

      this.button1Left = 80;
      this.button1Top = 0;
      this.button2Left = 20;
      this.button2Top = 80;

      this.button3Left = 20;
      this.button3Bottom = 80;
    } else {
      this.positionX = screenWidth - 32; // Snap to right (assuming button width is 32px)

      this.button1Right = 80;
      this.button1Left = 0;
      this.button2Left = 0;
      this.button2Right = 20;
      this.button3Left = 0;
      this.button3Right = 20;
    }
  }
}
